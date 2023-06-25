using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AteliwareAmazonia.Models;
using AteliwareAmazonia.Services;

namespace AteliwareAmazonia.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private ICoordinateService _coordinateService;
    private static List<string> _history = new List<string>();

    public HomeController(ILogger<HomeController> logger, ICoordinateService coordinateService)
    {
        _logger = logger;
        _coordinateService = coordinateService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<ShortestDestinationResponse> ShortestDestination(ShortestDestinationViewModel dataDto)
    {
        var start = dataDto.Start.ToUpper();
        var pickUp = dataDto.PickUp.ToUpper();
        var destination = dataDto.Destination.ToUpper();

        var route = await _coordinateService.CalculateRoute(start, pickUp, destination);
        if (!string.IsNullOrEmpty(route.Errors))
            return new ShortestDestinationResponse() { Errors = route.Errors };

        var routeResponse = new ShortestDestinationResponse();
        AddIntoHistory(start, pickUp, destination, route.Time);
        routeResponse.RouteDescription = $"The set delivery will have the route {RouteLinksToString(route.Links)}, and will take {route.Time} seconds to be delivered as fast as possible.";
        routeResponse.History = _history.ToArray();
        return routeResponse;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    string RouteLinksToString(string[] links)
    {
        var strRoute = new System.Text.StringBuilder();
        for (int i = 0; i < links.Length - 1; i++)
        {
            strRoute.Append($"{links[i]}-{links[i + 1]}+");
        }
        return strRoute.ToString().Remove(strRoute.Length - 1, 1);
    }

    void AddIntoHistory(string start, string pickUp, string destination, float time)
    {
        _history.Insert(0, $"- From {start}, picking-up at {pickUp} to {destination} in {time} seconds");
        if (_history.Count > 10)
            _history.RemoveAt(_history.Count - 1);
    }
}
