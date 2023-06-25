using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AteliwareAmazonia.Models;
using AteliwareAmazonia.Services;

namespace AteliwareAmazonia.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private CoordinateService _coordinateService;

    public HomeController(ILogger<HomeController> logger, CoordinateService coordinateService)
    {
        _logger = logger;
        _coordinateService = coordinateService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<RouteDto> ShortestDestination([FromBody] ShortestDestinationDto dataDto)
    {
        return await _coordinateService.CalculateRoute(dataDto.Start, dataDto.PickUp, dataDto.Destination);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
