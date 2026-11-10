namespace FragranceInfo.Helpers;

public sealed record NoteTag(string Name, string Family);

public static class NotePalette
{
    private static readonly (string Family, string[] Keywords)[] Families =
    [
        ("floral", ["rose", "jasmine", "iris", "violet", "lily", "tuberose", "peony", "ylang", "magnolia", "orchid", "flower", "blossom", "floral"]),
        ("citrus", ["bergamot", "lemon", "grapefruit", "orange", "mandarin", "lime", "yuzu", "neroli", "citrus"]),
        ("fruity", ["apple", "pear", "peach", "plum", "berry", "berries", "cassis", "blackcurrant", "fig", "fruit"]),
        ("sweet", ["vanilla", "tonka", "coffee", "caramel", "honey", "chocolate", "cocoa", "praline", "almond", "sugar"]),
        ("smoky", ["tobacco", "incense", "leather", "smoke", "myrrh", "labdanum", "resin", "benzoin"]),
        ("spicy", ["pepper", "ginger", "cinnamon", "cardamom", "clove", "nutmeg", "saffron", "spice"]),
        ("woody", ["cedar", "sandalwood", "vetiver", "oud", "birch", "patchouli", "guaiac", "cypress", "pine", "wood"]),
        ("green", ["green", "grass", "tea", "mint", "basil", "sage", "lavender", "galbanum", "moss", "leaf", "herb"]),
        ("musky", ["musk", "amber", "ambroxan", "powder"]),
        ("fresh", ["marine", "aquatic", "sea", "ozone", "salt", "water", "fresh"]),
    ];

    public static IReadOnlyList<NoteTag> Split(string? notes)
    {
        if (string.IsNullOrWhiteSpace(notes))
        {
            return [];
        }

        return notes
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(note => new NoteTag(char.ToUpper(note[0]) + note[1..], FamilyOf(note)))
            .ToList();
    }

    public static string FamilyOf(string note)
    {
        var text = note.ToLowerInvariant();
        foreach (var (family, keywords) in Families)
        {
            if (keywords.Any(text.Contains))
            {
                return family;
            }
        }

        return "other";
    }
}
