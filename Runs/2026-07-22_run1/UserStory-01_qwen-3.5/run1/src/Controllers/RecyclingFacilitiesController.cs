using Implementation.Models;
using Implementation.Services;
using Microsoft.AspNetCore.Mvc;

namespace Implementation.Controllers;

[Route("api/recycling-facilities")]
[ApiController]
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
        var facilities = _service.GetNearbyRecyclingFacilities(zipCode);
        return Ok(facilities);
    }
}