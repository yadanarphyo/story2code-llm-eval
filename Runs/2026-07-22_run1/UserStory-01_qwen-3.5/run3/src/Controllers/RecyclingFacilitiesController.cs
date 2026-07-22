using Microsoft.AspNetCore.Mvc;
using Implementation.Services;

namespace Implementation.Controllers
{
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
        public IActionResult GetFacilities([FromQuery] string zipCode)
        {
            var facilities = _service.GetNearbyRecyclingFacilities(zipCode);
            return Ok(facilities);
        }
    }
}