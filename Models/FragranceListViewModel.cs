namespace FragranceInfo.Models;

public class FragranceListViewModel
{
    public FragranceFilter Filter { get; set; } = new();

    public List<Fragrance> Fragrances { get; set; } = [];

    public List<string> Brands { get; set; } = [];
}
