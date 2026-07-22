// FILE: Implementation.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <AssemblyName>Implementation</AssemblyName>
    <RootNamespace>Implementation</RootNamespace>
  </PropertyGroup>
</Project>
```

// FILE: Models/RecyclingFacility.cs

```csharp
namespace Implementation.Models
{
    public class RecyclingFacility
    {
        public int FacilityId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public double DistanceInMiles { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
```

// FILE: Services/IGetNearbyRecyclingFacilitiesService.cs

```csharp
using Implementation.Models;

namespace Implementation.Services
{
    public interface IGetNearbyRecyclingFacilitiesService
    {
        IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode);
    }
}
```

// FILE: Services/GetNearbyRecyclingFacilitiesService.cs

```csharp
using Implementation.Models;

namespace Implementation.Services
{
    public class GetNearbyRecyclingFacilitiesService : IGetNearbyRecyclingFacilitiesService
    {
        private readonly List<RecyclingFacility> _facilities;

        public GetNearbyRecyclingFacilitiesService()
        {
            _facilities = new List<RecyclingFacility>
            {
                new RecyclingFacility
                {
                    FacilityId = 1,
                    Name = "Green Earth Recycling",
                    Address = "123 Main St",
                    City = "Springfield",
                    State = "IL",
                    ZipCode = "62701",
                    DistanceInMiles = 0.8,
                    PhoneNumber = "555-1234"
                },
                new RecyclingFacility
                {
                    FacilityId = 2,
                    Name = "Metro Recycling Center",
                    Address = "456 Oak Ave",
                    City = "Springfield",
                    State = "IL",
                    ZipCode = "62701",
                    DistanceInMiles = 1.5,
                    PhoneNumber = "555-5678"
                },
                new RecyclingFacility
                {
                    FacilityId = 3,
                    Name = "Suburban Waste Solutions",
                    Address = "789 Pine Rd",
                    City = "Shelbyville",
                    State = "IL",
                    ZipCode = "62702",
                    DistanceInMiles = 3.2,
                    PhoneNumber = "555-9012"
                }
            };
        }

        public IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)
        {
            return _facilities
                .Where(f => f.ZipCode == zipCode)
                .OrderBy(f => f.FacilityId)
                .ToList();
        }
    }
}
```

// FILE: Controllers/RecyclingFacilitiesController.cs

```csharp
using Implementation.Models;
using Implementation.Services;
using Microsoft.AspNetCore.Mvc;

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
        public ActionResult<IEnumerable<RecyclingFacility>> Get([FromQuery] string zipCode)
        {
            var results = _service.GetNearbyRecyclingFacilities(zipCode);
            return Ok(results);
        }
    }
}
```

// FILE: Program.cs

```csharp
using Implementation.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IGetNearbyRecyclingFacilitiesService, GetNearbyRecyclingFacilitiesService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```