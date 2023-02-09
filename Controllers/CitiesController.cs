using Microsoft.AspNetCore.Mvc;
using CityInfoAPI.Models;

namespace CityInfoAPI.Controllers {
  [ApiController]
  [Route("api/cities")]
  public class CitiesController : ControllerBase {
    [HttpGet]
    public ActionResult<IEnumerable<CityDto>> GetCities() {

      return Ok(CitiesDataStore.Current.Cities); 
    }

    [HttpGet("{id}")]
    public ActionResult<CityDto> GetCity(int id) {
      var cityToReturn = CitiesDataStore.Current.Cities.Find(city => city.Id == id);

      if (cityToReturn == null) {
        return NotFound();
      }
      return Ok(cityToReturn);
    }
  }
}