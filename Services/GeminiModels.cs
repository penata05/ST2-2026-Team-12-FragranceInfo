namespace FragranceInfo.Services;

internal sealed record GeminiRequest(
    GeminiContent SystemInstruction,
    GeminiContent[] Contents,
    GeminiGenerationConfig GenerationConfig);

internal sealed record GeminiContent(string? Role, GeminiPart[]? Parts);

internal sealed record GeminiPart(string? Text, bool? Thought = null);

internal sealed record GeminiGenerationConfig(int MaxOutputTokens, GeminiThinkingConfig ThinkingConfig);

internal sealed record GeminiThinkingConfig(string ThinkingLevel);

internal sealed record GeminiResponse(GeminiCandidate[]? Candidates, GeminiPromptFeedback? PromptFeedback);

internal sealed record GeminiCandidate(GeminiContent? Content, string? FinishReason);

internal sealed record GeminiPromptFeedback(string? BlockReason);
