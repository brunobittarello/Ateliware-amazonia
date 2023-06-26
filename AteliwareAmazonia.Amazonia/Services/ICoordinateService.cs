using AteliwareAmazonia.Models;

namespace AteliwareAmazonia.Services;

public interface ICoordinateService {
    public Task<RouteDto> CalculateRoute(string start, string pickUp, string destination);
}