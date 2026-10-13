namespace FragranceInfo.Models;

public class FragranceFilter
{
    public string? Name { get; set; }

    public string? Brand { get; set; }

    public string? Gender { get; set; }

    public bool IsActive =>
        !string.IsNullOrWhiteSpace(Name) ||
        !string.IsNullOrWhiteSpace(Brand) ||
        !string.IsNullOrWhiteSpace(Gender);
}
