using System.Linq;
using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers {
  [Route("api/cities")]
  public class PointsOfInterestController : Controller {
    [HttpGet("{cityId}/pointsofinterest")]
    public IActionResult GetPointsOfInterest(int cityId) {
      var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);

      if (city == null) {
        return NotFound();
      }

      return Ok(city.PointsOfInterest);
    }

    [HttpGet("{cityId}/pointsofinterest/{id}", Name = "GetPointOfInterest")]
    public IActionResult GetPointOfInterest(int cityId, int id) {
      var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);

      if (city == null) {
        return NotFound();
      }

      var pointOfInterest = city.PointsOfInterest.FirstOrDefault(x => x.Id == id);

      if (pointOfInterest == null) {
        return NotFound();
      }

      return Ok(pointOfInterest);
    }

    [HttpPost("{cityId}/pointsofinterest")]
    public IActionResult CreatePointOfInterest(int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterest) {
      if (pointOfInterest == null || !ModelState.IsValid) {
        return BadRequest(ModelState);
      }

      if (pointOfInterest.Description == pointOfInterest.Name) {
        ModelState.AddModelError("Description", "The provided description should be different from the name.");
        return BadRequest(ModelState);
      }

      var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);

      if (city == null) {
        return NotFound();
      }

      var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(x => x.PointsOfInterest).Max(x => x.Id);
      var newPointOfInterest = new PointOfInterestDto {
        Id = ++maxPointOfInterestId,
        Name = pointOfInterest.Name,
        Description = pointOfInterest.Description
      };

      city.PointsOfInterest.Add(newPointOfInterest);

      return CreatedAtRoute("GetPointOfInterest", new {cityId, id = newPointOfInterest.Id}, newPointOfInterest);
    }

    [HttpPut("{cityId}/pointsofinterest/{id}")]
    public IActionResult UpdatePointsOfInterest(int cityId, int id,
      [FromBody] PointOfInterestForUpdateDto pointOfInterest) {
      if (pointOfInterest == null || !ModelState.IsValid) {
        return BadRequest(ModelState);
      }

      if (pointOfInterest.Description == pointOfInterest.Name) {
        ModelState.AddModelError("Description", "The provided description should be different from the name.");
        return BadRequest(ModelState);
      }

      var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);
      if (city == null) {
        return NotFound();
      }

      var existingPointOfInterest = city.PointsOfInterest.FirstOrDefault(x => x.Id == id);
      if (existingPointOfInterest == null) {
        return NotFound();
      }

      existingPointOfInterest.Name = pointOfInterest.Name;
      existingPointOfInterest.Description = pointOfInterest.Description;

      return NoContent();
    }

    [HttpPatch("{cityId}/pointsofinterest/{id}")]
    public IActionResult PartiallyUpdatePointsOfInterest(int cityId, int id,
      [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument) {
      if (patchDocument == null) {
        return BadRequest();
      }

      var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);
      if (city == null) {
        return NotFound();
      }

      var existingPointOfInterest = city.PointsOfInterest.FirstOrDefault(x => x.Id == id);
      if (existingPointOfInterest == null) {
        return NotFound();
      }

      var pointOfInterestToPatch = new PointOfInterestForUpdateDto {
        Name = existingPointOfInterest.Name,
        Description = existingPointOfInterest.Description
      };

      patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);
      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }

      if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name) {
        ModelState.AddModelError("Description", "The provided description should be different from the name.");
      }

      TryUpdateModelAsync(pointOfInterestToPatch);
      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }

      existingPointOfInterest.Name = pointOfInterestToPatch.Name;
      existingPointOfInterest.Description = pointOfInterestToPatch.Description;

      return NoContent();
    }

    [HttpDelete("{cityId}/pointsofinterest/{id}")]
    public IActionResult DeletePointOfInterest(int cityId, int id) {
      var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);
      if (city == null) {
        return NotFound();
      }

      var existingPointOfInterest = city.PointsOfInterest.FirstOrDefault(x => x.Id == id);
      if (existingPointOfInterest == null) {
        return NotFound();
      }

      city.PointsOfInterest.Remove(existingPointOfInterest);
      return NoContent();
    }
  }
}
