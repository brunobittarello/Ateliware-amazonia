using System.Net;
using System.Net.Http;
using System.Text.Json;
using AteliwareAmazonia.Models;

namespace AteliwareAmazonia.Services;

public class CoordinatesLoader
{
    const string SOURCE_URL = "https://mocki.io/v1/10404696-fd43-4481-a7ed-f9369073252f";
    private readonly ILogger _logger;

    public CoordinatesLoader(ILogger<CoordinatesLoader> logger)
    {
        _logger = logger;
    }

    public async Task<List<Coordinate>?> Load()
    {
        var client = new HttpClient();
        var response = await client.GetAsync(SOURCE_URL);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            LogResponse(response);
            return null;
        }
        try
        {
            var responseBody = await response.Content.ReadAsStreamAsync();
            var json = new StreamReader(responseBody).ReadToEnd();
            var result = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, float>>>(json);
            if (result == null)
                throw new Exception("Json parse resulted in a null object");

            return ConvertToCoordinates(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Coordinate loading failed");
            return null;
        }
    }

    List<Coordinate> ConvertToCoordinates(Dictionary<string, Dictionary<string, float>> dictionary)
    {
        var coordinates = new List<Coordinate>();
        foreach (var dictCoord in dictionary)
        {
            coordinates.Add(
                new Coordinate() { Name = dictCoord.Key, Links = dictCoord.Value }
            );
        }
        return coordinates;
    }

    async void LogResponse(HttpResponseMessage response)
    {
        var responseBody = await response.Content.ReadAsStreamAsync();
        var responseText = "";
        if (responseBody != null)
            responseText = new StreamReader(responseBody).ReadToEnd();
        _logger.LogInformation($"{response.StatusCode} {responseText}");
    }
}