using FragranceInfo.Data;
using FragranceInfo.Models;
using Microsoft.AspNetCore.Mvc;

namespace FragranceInfo.Controllers;

public class FragrancesController : Controller
{
    private readonly FragranceRepository _repository;

    public FragrancesController(FragranceRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> Index(FragranceFilter filter)
    {
        var model = new FragranceListViewModel
        {
            Filter = filter,
            Fragrances = await _repository.GetFilteredAsync(filter),
            Brands = await _repository.GetBrandsAsync(),
        };

        return View(model);
    }

    public IActionResult Create()
    {
        return View(new Fragrance());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Fragrance fragrance)
    {
        if (!ModelState.IsValid)
        {
            return View(fragrance);
        }

        await _repository.InsertAsync(fragrance);

        TempData["Message"] = $"{fragrance.Name.Trim()} by {fragrance.Brand.Trim()} was added.";
        return RedirectToAction(nameof(Index));
    }
}
