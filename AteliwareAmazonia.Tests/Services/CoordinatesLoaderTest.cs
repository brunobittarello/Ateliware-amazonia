using AteliwareAmazonia.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace AteliwareAmazonia.Tests;

public class CoordinatesLoaderTest
{
    [Fact(DisplayName = "Coordinate loader is loading from API")]
    public async void Loader_LoadsAllCoordinates()
    {
        //Arrange
        var loader = CreateLoader();

        //Act
        var coordinates = await loader.Load();

        //Assert
        Assert.Equal(64, coordinates.Count);
    }

    [Fact(DisplayName = "Coordinate loader fails to load from API because bad API url")]
    public async void Loader_FailToLoad()
    {
        //Arrange
        var loader = CreateLoader(false);

        //Act
        var coordinates = await loader.Load();

        //Assert
        Assert.True(coordinates == null);
    }

    CoordinatesLoader CreateLoader(bool withWorkingApi = true)
    {
        var apiUrl = withWorkingApi ? "https://mocki.io/v1/10404696-fd43-4481-a7ed-f9369073252f" : "";
        var inMemorySettings = new Dictionary<string, string> {
            {"RoutesApiUrl", apiUrl}
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        var logger = new Mock<ILogger<CoordinatesLoader>>();
        return new CoordinatesLoader(logger.Object, configuration);
    }
}