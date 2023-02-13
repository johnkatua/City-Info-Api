using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CityInfoAPI.Models;
using CityInfoAPI.Services;
using AutoMapper;

namespace CityInfoAPI.Controllers {
  [Route("api/cities")]
  [Authorize(Policy = "MustBeFromNairobi")]
  [ApiController]
  public class CitiesController : ControllerBase {
    private readonly ICityInfoRepository _cityInfoRepository;
    private readonly IMapper _mapper;

    public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
    {
      _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities() {
      var cityEntities = await _cityInfoRepository.GetCitiesAsync();
      // var results = new List<CityWithoutPointsOfInterestDto>();
      // foreach(var cityEntity in cityEntities) {
      //   results.Add(new CityWithoutPointsOfInterestDto {
      //     Id = cityEntity.Id,
      //     Description = cityEntity.Description,
      //     Name = cityEntity.Name
      //   });
      // }

      // mapping entities to cityWithPointsOfInterestDto, simplified version of the above code
      return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCity(int id, bool includePointsOfInterest = false) {
      var city = await _cityInfoRepository.GetCityAsync(id, includePointsOfInterest);
      if (city == null) {
        return NotFound();
      }

      if (includePointsOfInterest) {
        return Ok(_mapper.Map<CityDto>(city));
      }
      return  Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(city));
      // var cityToReturn = CitiesDataStore.Current.Cities.Find(city => city.Id == id);

      // if (cityToReturn == null) {
      //   return NotFound();
      // }
      // return Ok(cityToReturn);
    }
  }
}