using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FragranceInfo.Models;

public class AdvisorViewModel
{
    [Required(ErrorMessage = "Describe what you are looking for.")]
    [StringLength(500, ErrorMessage = "Keep the request under 500 characters.")]
    [Display(Name = "What are you looking for?")]
    public string Question { get; set; } = string.Empty;

    [BindNever]
    public string? Answer { get; set; }

    [BindNever]
    public string? Error { get; set; }

    [BindNever]
    public bool IsConfigured { get; set; }
}
