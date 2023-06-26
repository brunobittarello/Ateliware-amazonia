using System.ComponentModel.DataAnnotations;

namespace AteliwareAmazonia.Models;

public class ShortestDestinationViewModel
{
    [Required(ErrorMessage = "This field is required")]
    public string Start { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string PickUp { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string Destination { get; set; }
}