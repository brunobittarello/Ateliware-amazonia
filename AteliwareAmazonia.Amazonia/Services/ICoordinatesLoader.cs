using AteliwareAmazonia.Models;

namespace AteliwareAmazonia.Services;

public interface ICoordinatesLoader {
    public Task<List<Coordinate>> Load();
}