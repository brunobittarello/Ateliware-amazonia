using System.Linq;
using System.Text;
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
        var error = "";
        if (!ValidateRequisition(coordinates, start, pickUp, destination, out error))
        {
            return new RouteDto() { Errors = error };
        }

        var startToPickUp = ShortestWay(coordinates, start, pickUp);
        var pickUpToDestination = ShortestWay(coordinates, pickUp, destination);
        return new RouteDto()
        {
            Time = startToPickUp.Time + pickUpToDestination.Time,
            Links = MergeLinks(startToPickUp.Links, pickUpToDestination.Links)
        };

    }

    bool ValidateRequisition(List<Coordinate> coordinates, string start, string pickUp, string destination, out string error)
    {
        var isValid = true;
        var errors = new StringBuilder();
        if (!ValidateCoordinate(coordinates, start, "Start", errors))
            isValid = false;
        if (!ValidateCoordinate(coordinates, pickUp, "Pick-up", errors))
            isValid = false;
        if (!ValidateCoordinate(coordinates, destination, "Destination", errors))
            isValid = false;

        error = errors.ToString();
        return isValid;
    }

    bool ValidateCoordinate(List<Coordinate> coordinates, string coordinate, string name, StringBuilder errors)
    {
        if (coordinates.Any(c => c.Name == coordinate))
            return true;
        errors.Append($"\n{name} coordinate invalid");
        return false;
    }

    RouteDto ShortestWay(List<Coordinate> coordinates, string start, string destination)
    {
        RouteDto bestRoute = null;
        var startCoord = coordinates.FirstOrDefault(c => c.Name == start);
        var visitedCoord = new List<string>();
        var toVisitCoord = new Queue<RouteDto>();
        visitedCoord.Add(startCoord.Name);
        foreach (var link in startCoord.Links)
            toVisitCoord.Enqueue(new RouteDto() { Time = link.Value, Last = link.Key, Links = new string[] { startCoord.Name, link.Key } });

        while (toVisitCoord.Count > 0)
        {
            var curr = toVisitCoord.Dequeue();
            var currCoord = coordinates.FirstOrDefault(c => c.Name == curr.Last);
            visitedCoord.Add(curr.Last);

            if (curr.Last == destination)
            {
                bestRoute = curr;
                continue;
            }

            foreach (var link in currCoord.Links)
            {
                if (bestRoute == null || bestRoute.Time > curr.Time + link.Value)
                    toVisitCoord.Enqueue(AddCoord(curr, link.Key, link.Value));
            }
        }

        return bestRoute;
    }

    RouteDto AddCoord(RouteDto current, string coord, float time)
    {
        return new RouteDto()
        {
            Last = coord,
            Time = current.Time + time,
            Links = current.Links.Union(new string[] { coord }).ToArray()
        };
    }

    string[] MergeLinks(string[] links1, string[] links2)
    {
        var list = new List<string>(links1);
        list.AddRange(links2);
        list.RemoveAt(links1.Length);
        return list.ToArray();
    }
}