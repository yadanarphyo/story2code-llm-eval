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
    public ActionResult<IEnumerable<RecyclingFacility>> Get([FromQuery] string zipCode)
    {
        var result = _service.GetNearbyRecyclingFacilities(zipCode);
        return Ok(result);
    }
}