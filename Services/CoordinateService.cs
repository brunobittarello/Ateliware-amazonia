using System.Net;
using AteliwareAmazonia.Models;

namespace AteliwareAmazonia.Services;

public class CoordinateService
{
    private readonly ILogger _logger;
    private readonly CoordinatesLoader _loader;

    public CoordinateService(ILogger<CoordinateService> logger, CoordinatesLoader loader)
    {
        _logger = logger;
        _loader = loader;
    }

    public async Task<RouteDto> CalculateRoute(string start, string pickUp, string destination)
    {
        _logger.LogInformation($"start={start} pickup={pickUp} dest={destination}");
        var coordinates = await _loader.Load();

        var startToPickUp = ShortestWay(coordinates, start, pickUp);
        var pickUpToDestination = ShortestWay(coordinates, pickUp, destination);
        return new RouteDto() { 
            Time = startToPickUp.Time + pickUpToDestination.Time,
            Links = MergeLinks(startToPickUp.Links, pickUpToDestination.Links) };

    }

    RouteDto ShortestWay(List<Coordinate> coordinates, string start, string destination)
    {
        return new RouteDto() { Time = 88, Links = new string[] { start, destination } };
    }

    string[] MergeLinks(string[] links1, string[] links2)
    {
        var list = new List<string>(links1);
        list.AddRange(links2);
        list.RemoveAt(links1.Length);
        return list.ToArray();
    }
}