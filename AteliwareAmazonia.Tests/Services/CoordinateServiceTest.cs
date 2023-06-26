using AteliwareAmazonia.Models;
using AteliwareAmazonia.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace AteliwareAmazonia.Tests;

public class CoordinateServiceTest
{
    [Fact(DisplayName = "Route should calculate the right time and route")]
    public async void CoordService_CalculateRightRouteValues()
    {
        //Arrange
        var service = CreateCoordinateService();
        var start = "A1";
        var pickUp = "A2";
        var destination = "A3";

        //Act
        var route = await service.CalculateRoute(start, pickUp, destination);

        //Assert
        Assert.Equal(10, route.Time);
        Assert.Equal(3, route.Links.Length);
        Assert.Contains<string>(destination, route.Links);
    }

    [Fact(DisplayName = "Route should be have 0 seconds and 1 step when the 3 coords are the same")]
    public async void CoordService_SameCoords()
    {
        //Arrange
        var service = CreateCoordinateService();
        var start = "A1";
        var pickUp = "A1";
        var destination = "A1";

        //Act
        var route = await service.CalculateRoute(start, pickUp, destination);

        //Assert
        Assert.Equal(0, route.Time);
        Assert.Equal(2, route.Links.Length);
        Assert.Contains<string>(destination, route.Links);
    }

    [Fact(DisplayName = "Route should have error when destination is unreachablee")]
    public async void CoordService_DestinationUnreachable()
    {
        //Arrange
        var service = CreateCoordinateService();
        var start = "A1";
        var pickUp = "A1";
        var destination = "A9";

        //Act
        var route = await service.CalculateRoute(start, pickUp, destination);

        //Assert
        Assert.Equal("\nDestination coordinate invalid", route.Errors);
    }

    [Fact(DisplayName = "Route should take the shortest route")]
    public async void CoordService_Shortest()
    {
        //Arrange
        var coordinates = new List<Coordinate>();
        coordinates.Add(new Coordinate() { Name = "A1", Links = new Dictionary<string, float>() { { "A2", 5 }, { "B1", 5 } } });
        coordinates.Add(new Coordinate() { Name = "A2", Links = new Dictionary<string, float>() { { "A1", 2 }, { "A3", 5 } } });
        coordinates.Add(new Coordinate() { Name = "A3", Links = new Dictionary<string, float>() { { "A2", 2 }, } });
        coordinates.Add(new Coordinate() { Name = "B1", Links = new Dictionary<string, float>() { { "A3", 1 }, { "B2", 5 }, { "C1", 6 } } });
        coordinates.Add(new Coordinate() { Name = "B2", Links = new Dictionary<string, float>() { { "Y8", 8 } } });
        coordinates.Add(new Coordinate() { Name = "C1", Links = new Dictionary<string, float>() { { "Y8", 1 } } });
        coordinates.Add(new Coordinate() { Name = "Y8", Links = new Dictionary<string, float>() { { "C1", 1 } } });
        var service = CreateCoordinateService(coordinates);
        var start = "A1";
        var pickUp = "A1";
        var destination = "Y8";

        //Act
        var route = await service.CalculateRoute(start, pickUp, destination);

        //Assert
        Assert.Equal(null, route.Errors);
        Assert.Equal(12, route.Time);
        Assert.Equal(4, route.Links.Length);
        Assert.Contains<string>("C1", route.Links);
    }

    CoordinateService CreateCoordinateService()
    {
        var coordinates = new List<Coordinate>();
        coordinates.Add(new Coordinate() { Name = "A1", Links = new Dictionary<string, float>() { { "A2", 5 } } });
        coordinates.Add(new Coordinate() { Name = "A2", Links = new Dictionary<string, float>() { { "A1", 2 }, { "A3", 5 } } });
        coordinates.Add(new Coordinate() { Name = "A3", Links = new Dictionary<string, float>() { { "A2", 2 }, } });
        return CreateCoordinateService(coordinates);
    }

    CoordinateService CreateCoordinateService(List<Coordinate> coordinates)
    {
        var logger = new Mock<ILogger<CoordinateService>>();
        var loader = new Mock<ICoordinatesLoader>();
        loader.Setup(x => x.Load()).Returns(Task.FromResult(coordinates));
        return new CoordinateService(logger.Object, loader.Object);
    }
}