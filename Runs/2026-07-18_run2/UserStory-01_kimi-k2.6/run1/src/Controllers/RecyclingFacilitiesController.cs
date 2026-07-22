using Implementation.Models;
using Implementation.Services;
using Microsoft.AspNetCore.Mvc;

namespace Implementation.Controllers;

[ApiController]
[Route("api/recycling-facilities")]
public class RecyclingFacilitiesController : ControllerBase
{
    private readonly IGetNearbyRecyclingFacilitiesService _service;

    public RecyclingFacilitiesController(IGetNearbyRecyclingFacilitiesService service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<IEnumerable<RecyclingFacility>> GetNearbyRecyclingFacilities([FromQuery] string zipCode)
    {
        if (string.IsNullOrWhiteSpace(zipCode))
        {
            return BadRequest("zipCode is required.");
        }

        var results = _service.GetNearbyRecyclingFacilities(zipCode);
        return Ok(results);
    }
}