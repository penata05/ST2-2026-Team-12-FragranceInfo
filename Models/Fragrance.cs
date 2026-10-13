using System.ComponentModel.DataAnnotations;

namespace FragranceInfo.Models;

public class Fragrance : IValidatableObject
{
    public static readonly string[] Genders = ["Male", "Female", "Unisex"];

    public static readonly string[] Concentrations =
        ["Eau de Cologne", "Eau de Toilette", "Eau de Parfum", "Parfum", "Extrait de Parfum"];

    public int Id { get; set; }

    [Required(ErrorMessage = "Enter the name of the fragrance.")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Enter the brand.")]
    [StringLength(100)]
    public string Brand { get; set; } = string.Empty;

    [Display(Name = "Release year")]
    [Range(1700, 2100, ErrorMessage = "Enter a year between 1700 and 2100.")]
    public int? ReleaseYear { get; set; }

    [Required(ErrorMessage = "Choose a gender.")]
    public string Gender { get; set; } = string.Empty;

    [Required(ErrorMessage = "Choose a concentration.")]
    public string Concentration { get; set; } = string.Empty;

    [Display(Name = "Main notes")]
    [StringLength(500)]
    public string? Notes { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!Genders.Contains(Gender))
        {
            yield return new ValidationResult("Choose a gender from the list.", [nameof(Gender)]);
        }

        if (!Concentrations.Contains(Concentration))
        {
            yield return new ValidationResult("Choose a concentration from the list.", [nameof(Concentration)]);
        }
    }
}
