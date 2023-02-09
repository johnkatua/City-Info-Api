// using CityInfoAPI.Models;
using CityInfoAPI.Models;

namespace CityInfoAPI {
  public class CitiesDataStore {
    public List<CityDto> Cities { get; set; }
    public static CitiesDataStore Current { get; } = new CitiesDataStore();
    public CitiesDataStore() {
      Cities = new List<CityDto>() {
        new CityDto() {
          Id = 1,
          Name = "Mombasa",
          Description = "Coastal City in Kenya",
          PointsOfInterest = new List<PointOfInterestDto>() {
            new PointOfInterestDto() {
              Id = 1,
              Name = "Ukunda",
              Description = "The ensuite for tourists"
            },
            new PointOfInterestDto() {
              Id = 2,
              Name = "Fort Jesus",
              Description = "Land mark for slave trade."
            },
          }
        },
        new CityDto() {
          Id = 2,
          Name = "Nairobi",
          Description = "Capital city of Kenya",
          PointsOfInterest = new List<PointOfInterestDto>() {
            new PointOfInterestDto() {
              Id = 1,
              Name = "Limuru",
              Description = "Limuru Golf Club"
            },
            new PointOfInterestDto() {
              Id = 2,
              Name = "KICC",
              Description = "Internation Conference Center"
            },
          }
        },
        new CityDto() {
          Id = 3,
          Name = "Kisumu",
          Description = "The city surrounded by Lake Victoria",
          PointsOfInterest = new List<PointOfInterestDto>() {
            new PointOfInterestDto() {
              Id = 1,
              Name = "Namanga",
              Description = "Heart of Mount Elgon"
            },
            new PointOfInterestDto() {
              Id = 2,
              Name = "Maseno",
              Description = "Best National School"
            },
          }
        },
      };
    }
  }
}