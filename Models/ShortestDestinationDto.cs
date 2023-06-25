using System.ComponentModel.DataAnnotations;

namespace AteliwareAmazonia.Models;

public class ShortestDestinationDto
{
    [Required(ErrorMessage = "This field is required")]
    public string Start { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string PickUp { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string Destination { get; set; }
}