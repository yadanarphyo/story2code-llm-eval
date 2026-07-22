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

// FILE: Program.cs
```csharp
using Implementation.Models;
using Implementation.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register the service implementation.
builder.Services.AddTransient<IGetNearbyRecyclingFacilitiesService, GetNearbyRecyclingFacilitiesService>();

// Register the data dependency required by the service constructor.
// For the live application, we provide an empty list. 
// The unit tests bypass DI and instantiate the service directly with test data.
builder.Services.AddSingleton<IEnumerable<RecyclingFacility>>(new List<RecyclingFacility>());

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

app.Run();
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
using System.Collections.Generic;
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
using System;
using System.Collections.Generic;
using System.Linq;
using Implementation.Models;

namespace Implementation.Services
{
    public class GetNearbyRecyclingFacilitiesService : IGetNearbyRecyclingFacilitiesService
    {
        private readonly IEnumerable<RecyclingFacility> _facilities;

        public GetNearbyRecyclingFacilitiesService(IEnumerable<RecyclingFacility> facilities)
        {
            _facilities = facilities ?? throw new ArgumentNullException(nameof(facilities));
        }

        public IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)
        {
            if (string.IsNullOrWhiteSpace(zipCode))
            {
                throw new ArgumentException("Zip code cannot be null, empty, or whitespace.", nameof(zipCode));
            }

            var results = _facilities
                .Where(f => f.ZipCode == zipCode)
                .ToList();

            return results;
        }
    }
}
```

// FILE: Controllers/RecyclingFacilitiesController.cs
```csharp
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
```