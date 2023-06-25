using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AteliwareAmazonia.Models;
using AteliwareAmazonia.Services;

namespace AteliwareAmazonia.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private CoordinatesLoader _loader;

    public HomeController(ILogger<HomeController> logger, CoordinatesLoader loader)
    {
        _logger = logger;
        _loader = loader;
    }

    public async Task<IActionResult> Index()
    {
        await _loader.Load();
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
