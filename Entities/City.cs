using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CityInfoAPI.Models;

namespace CityInfoAPI.Entities {
  public class City {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    [MaxLength(200)]
    public string? Description { get; set; }
    public ICollection<PointOfInterestDto> PointsOfInterest { get; set; } = new List<PointOfInterestDto>(); 

    public City(string name) {
      Name = name;
    }
  }
}