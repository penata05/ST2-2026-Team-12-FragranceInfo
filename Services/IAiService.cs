namespace FragranceInfo.Services;

public interface IAiService
{
    bool IsConfigured { get; }

    Task<string> GenerateAsync(string instructions, string prompt, CancellationToken cancellationToken = default);
}
