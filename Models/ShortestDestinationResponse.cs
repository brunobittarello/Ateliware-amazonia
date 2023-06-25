namespace AteliwareAmazonia.Models;

public class ShortestDestinationResponse
{
    public string RouteDescription { get; set; }
    public string[] History { get; set; }
    public string Error { get; set; }
}