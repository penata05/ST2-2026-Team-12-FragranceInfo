using System.ComponentModel.DataAnnotations;

namespace FragranceInfo.Services;

public class GeminiOptions
{
    public const string SectionName = "Gemini";

    public string ApiKey { get; set; } = string.Empty;

    [Required]
    public string Model { get; set; } = "gemini-3.8-flash";

    [Required]
    [Url]
    public string BaseUrl { get; set; } = "https://generativelanguage.googleapis.com/v1beta/";

    [Range(5, 120)]
    public int TimeoutSeconds { get; set; } = 30;

    [Range(256, 8192)]
    public int MaxOutputTokens { get; set; } = 2048;

    [Required]
    [RegularExpression("^(low|medium|high)$")]
    public string ThinkingLevel { get; set; } = "low";
}
