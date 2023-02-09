using System.ComponentModel.DataAnnotations;

namespace CityInfoAPI.Models {
  public class PointOfInterestForUpdateDto {
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(200)]
    public string? Description { get; set; }
  }
}