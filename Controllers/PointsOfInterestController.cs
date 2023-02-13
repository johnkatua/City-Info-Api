using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using CityInfoAPI.Models;
using CityInfoAPI.Services;
using AutoMapper;

namespace CityInfoAPI.Controllers {
  [Route("api/cities/{cityId}/pointsOfInterest")] 
  [ApiController]
  public class PointsOfInterestController : ControllerBase {
    private readonly ILogger<PointsOfInterestController> _logger;
    private readonly IMailService _mailService;
    private readonly IMapper _mapper;

    private readonly ICityInfoRepository _cityInfoRepository;
    public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, ICityInfoRepository cityInfoRepository, IMapper mapper) {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
      _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(mailService));
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId) {
      var cityName = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;

      if (!await _cityInfoRepository.CityNameMatchesCityId(cityName, cityId)) {
        return Forbid();
      }
      if(!await _cityInfoRepository.CityExistsAsync(cityId)) {
        _logger.LogInformation($"City with an id {cityId} was not found");
        return NotFound();
      }
      var pointsOfInterest = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);
      return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterest));
      // try
      // { 
      //   var city = CitiesDataStore.Current.Cities.Find(city => city.Id == cityId);

      //   if (city == null) {
      //     _logger.LogInformation($"City with an id of {cityId} wasn't found");
      //     return NotFound();
      //   }
      //   return Ok(city.PointsOfInterest);
      // }
      // catch (Exception ex)
      // {
      //   _logger.LogCritical($"Something is really  off:", ex);
      //   return StatusCode(500, "A problem happened while handling the request");
      // }
    }

    [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
    public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId) {
      if (!await _cityInfoRepository.CityExistsAsync(cityId)) {
        return NotFound();
      }

      var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

      if (pointOfInterest == null) {
        return NotFound();
      }

      return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
      // var city = CitiesDataStore.Current.Cities.Find(c => c.Id == cityId);
      // if(city == null) {
      //   // _logger.LogInformation($"City with an id of {cityId} wasn't found");
      //   return NotFound();
      // }

      // // find point of interest
      // var pointOfInterest = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);

      // if (pointOfInterest == null) {
      //   return NotFound();
      // }

      // return Ok(pointOfInterest);
    }

    [HttpPost]
    public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest (int cityId, PointOfInterestForCreationDto pointOfInterest) {
      if (!await _cityInfoRepository.CityExistsAsync(cityId)) {
        return NotFound();
      }

      // var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);
      // var city = CitiesDataStore.Current.Cities.Find(c => c.Id == cityId);
      // if (city == null) {
      //   return NotFound();
      // }

      // Calculate the highest id of points of interests...to be reimplemented[not idle]
      // var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

      var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

      // var finalPointOfInterest = new PointOfInterestDto() {
      //   Id = ++maxPointOfInterestId,
      //   Name = pointOfInterest.Name,
      //   Description = pointOfInterest.Description
      // };

      // city.PointsOfInterest.Add(finalPointOfInterest);

       _cityInfoRepository.AddPointOfInterestForCityAsync(finalPointOfInterest);

      await _cityInfoRepository.SaveChangesAsync();

      var createdPointOfInterestToReturn = _mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);

      return CreatedAtRoute("GetPointOfInterest", new {
        cityId = cityId,
        pointOfInterestId = createdPointOfInterestToReturn.Id
      }, createdPointOfInterestToReturn);
    }

    [HttpPut("{pointOfInterestId}")]
    public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest) {
      // var city = CitiesDataStore.Current.Cities.Find(c => c.Id == cityId);
      // if (city == null) {
      //   return NotFound();
      // }
      if (!await _cityInfoRepository.CityExistsAsync(cityId))
      {
        return NotFound();
      }

      // Find point of interest
      // var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == cityId);
      // if (pointOfInterestFromStore == null) {
      //   return NotFound();
      // }
      var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

      if (pointOfInterestEntity == null)
      {
        return NotFound();
      }

      _mapper.Map(pointOfInterest, pointOfInterestEntity);

      await _cityInfoRepository.SaveChangesAsync();

      // pointOfInterestFromStore.Name = pointOfInterest.Name;
      // pointOfInterestFromStore.Description = pointOfInterest.Description;

      return NoContent();
    }

    [HttpPatch("{pointOfInterestId}")]
    public ActionResult PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument) {
      var city = CitiesDataStore.Current.Cities.Find(c => c.Id == cityId);

      if (city == null) {
        return NotFound();
      }

      var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);

      if (pointOfInterestFromStore == null) {
        return NotFound();
      }

      var pointOfInterestToPatch = new PointOfInterestForUpdateDto() {
        Name = pointOfInterestFromStore.Name,
        Description = pointOfInterestFromStore.Description
      };

      patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }

      // Manually validate errors after using json patch document
      if (!TryValidateModel(pointOfInterestToPatch)) {
        return BadRequest(ModelState);
      }

      pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
      pointOfInterestFromStore.Description =pointOfInterestToPatch.Description;

      return NoContent();
    }

    [HttpDelete("{pointOfInterestId}")]
    public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId) {
      // var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
      // if (city == null) {
      //   return NotFound();
      // }

      if (!await _cityInfoRepository.CityExistsAsync(cityId)) {
        return NotFound();
      }

      // var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);

      // if (pointOfInterestFromStore == null) {
      //   return NotFound();
      // }

      var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

      if (pointOfInterestEntity == null) {
        return NotFound();
      }

      _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);

      await _cityInfoRepository.SaveChangesAsync();

      // city.PointsOfInterest.Remove(pointOfInterestFromStore);
      _mailService.Send("Point of interest deleted", $"Point of interest with an id of {pointOfInterestEntity.Id} was deleted");
      return NoContent();
    }
  }
}