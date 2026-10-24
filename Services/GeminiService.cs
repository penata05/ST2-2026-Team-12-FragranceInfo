using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace FragranceInfo.Services;

public class GeminiService : IAiService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly HttpClient _httpClient;
    private readonly GeminiOptions _options;
    private readonly ILogger<GeminiService> _logger;

    public GeminiService(HttpClient httpClient, IOptions<GeminiOptions> options, ILogger<GeminiService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public bool IsConfigured => !string.IsNullOrWhiteSpace(_options.ApiKey);

    public async Task<string> GenerateAsync(string instructions, string prompt, CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            throw new AiServiceException("The AI advisor is not set up: the Gemini API key is missing.");
        }

        var body = new GeminiRequest(
            new GeminiContent(null, [new GeminiPart(instructions)]),
            [new GeminiContent("user", [new GeminiPart(prompt)])],
            new GeminiGenerationConfig(_options.MaxOutputTokens, new GeminiThinkingConfig(_options.ThinkingLevel)));

        using var request = new HttpRequestMessage(HttpMethod.Post, $"models/{_options.Model}:generateContent")
        {
            Content = new StringContent(JsonSerializer.Serialize(body, JsonOptions), Encoding.UTF8, "application/json"),
        };
        request.Headers.Add("x-goog-api-key", _options.ApiKey);

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(request, cancellationToken);
        }
        catch (TaskCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(exception, "Gemini did not answer within {Seconds} seconds.", _options.TimeoutSeconds);
            throw new AiServiceException("The AI took too long to answer. Try again.", exception);
        }
        catch (HttpRequestException exception)
        {
            _logger.LogWarning(exception, "Could not reach Gemini.");
            throw new AiServiceException("Could not reach the AI service. Check your internet connection.", exception);
        }

        using (response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Gemini returned {StatusCode}: {Error}", (int)response.StatusCode, Shorten(error));
                throw new AiServiceException(MessageFor(response.StatusCode));
            }

            GeminiResponse? result;
            try
            {
                result = await response.Content.ReadFromJsonAsync<GeminiResponse>(JsonOptions, cancellationToken);
            }
            catch (JsonException exception)
            {
                _logger.LogWarning(exception, "Gemini returned a response that could not be read.");
                throw new AiServiceException("The AI returned an answer that could not be read. Try again.", exception);
            }

            return ExtractText(result);
        }
    }

    private string ExtractText(GeminiResponse? result)
    {
        if (result?.PromptFeedback?.BlockReason is { } blockReason)
        {
            _logger.LogInformation("Gemini blocked the prompt: {Reason}", blockReason);
            throw new AiServiceException("The request was blocked by the AI safety filters. Try rephrasing it.");
        }

        var candidate = result?.Candidates?.FirstOrDefault();
        var text = string.Concat(
            candidate?.Content?.Parts?
                .Where(part => part.Thought != true)
                .Select(part => part.Text) ?? []).Trim();

        if (text.Length > 0)
        {
            return text;
        }

        _logger.LogWarning("Gemini returned no text. Finish reason: {Reason}", candidate?.FinishReason);
        throw new AiServiceException(candidate?.FinishReason switch
        {
            "SAFETY" or "PROHIBITED_CONTENT" or "BLOCKLIST" or "SPII" => "The answer was blocked by the AI safety filters. Try rephrasing the request.",
            "MAX_TOKENS" => "The answer was too long. Try a more specific request.",
            _ => "The AI returned an empty answer. Try again.",
        });
    }

    private static string MessageFor(HttpStatusCode statusCode) => statusCode switch
    {
        HttpStatusCode.TooManyRequests => "The AI usage limit was reached. Wait a minute and try again.",
        HttpStatusCode.BadRequest or HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden =>
            "The AI service rejected the request. Check that the Gemini API key is correct.",
        HttpStatusCode.NotFound => "The Gemini model in appsettings.json was not found.",
        _ => "The AI service is not available right now. Try again later.",
    };

    private static string Shorten(string text) => text.Length <= 500 ? text : text[..500] + "...";
}
