using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using CityInfoAPI.Models;
using CityInfoAPI.Services;

namespace CityInfoAPI.Controllers {
  [Route("api/cities/{cityId}/pointsOfInterest")]
  [ApiController]
  public class PointsOfInterestController : ControllerBase {
    private readonly ILogger<PointsOfInterestController> _logger;
    private readonly IMailService _mailService;
    public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService) {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
    }

    [HttpGet]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId) {
      try
      { 
        // throw new Exception("Testing");
        var city = CitiesDataStore.Current.Cities.Find(city => city.Id == cityId);

        if (city == null) {
          _logger.LogInformation($"City with an id of {cityId} wasn't found");
          return NotFound();
        }
        return Ok(city.PointsOfInterest);
      }
      catch (Exception ex)
      {
        _logger.LogCritical($"Something is really  off:", ex);
        return StatusCode(500, "A problem happened while handling the request");
      }
    }

    [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
    public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId){
      var city = CitiesDataStore.Current.Cities.Find(c => c.Id == cityId);
      if(city == null) {
        // _logger.LogInformation($"City with an id of {cityId} wasn't found");
        return NotFound();
      }

      // find point of interest
      var pointOfInterest = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);

      if (pointOfInterest == null) {
        return NotFound();
      }

      return Ok(pointOfInterest);
    }

    [HttpPost]
    public ActionResult<PointOfInterestForCreationDto> CreatePointOfInterest (int cityId, PointOfInterestForCreationDto pointOfInterest) {
      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }
      var city = CitiesDataStore.Current.Cities.Find(c => c.Id == cityId);
      if (city == null) {
        return NotFound();
      }

      // Calculate the highest id of points of interests...to be reimplemented[not idle]
      var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

      var finalPointOfInterest = new PointOfInterestDto() {
        Id = ++maxPointOfInterestId,
        Name = pointOfInterest.Name,
        Description = pointOfInterest.Description
      };

      city.PointsOfInterest.Add(finalPointOfInterest);

      return CreatedAtRoute("GetPointOfInterest", new {
        cityId = cityId,
        pointOfInterestId = finalPointOfInterest.Id
      }, finalPointOfInterest);
    }

    [HttpPut("{pointOfInterestId}")]
    public ActionResult UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest) {
      var city = CitiesDataStore.Current.Cities.Find(c => c.Id == cityId);
      if (city == null) {
        return NotFound();
      }

      // Find point of interest
      var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == cityId);

      if (pointOfInterestFromStore == null) {
        return NotFound();
      }

      pointOfInterestFromStore.Name = pointOfInterest.Name;
      pointOfInterestFromStore.Description = pointOfInterest.Description;

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
    public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId) {
      var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
      if (city == null) {
        return NotFound();
      }

      var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);

      if (pointOfInterestFromStore == null) {
        return NotFound();
      }

      city.PointsOfInterest.Remove(pointOfInterestFromStore);
      _mailService.Send("Point of interest deleted", $"Point of interest with an id of {pointOfInterestFromStore.Id} was deleted");
      return NoContent();
    }
  }
}