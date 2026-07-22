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
using Implementation.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<IGetNearbyRecyclingFacilitiesService, GetNearbyRecyclingFacilitiesService>();

var app = builder.Build();

app.MapControllers();

app.Run();
```

// FILE: Models/RecyclingFacility.cs
```csharp
namespace Implementation.Models;

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
```

// FILE: Services/IGetNearbyRecyclingFacilitiesService.cs
```csharp
using Implementation.Models;

namespace Implementation.Services;

public interface IGetNearbyRecyclingFacilitiesService
{
    IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode);
}
```

// FILE: Services/GetNearbyRecyclingFacilitiesService.cs
```csharp
using Implementation.Models;

namespace Implementation.Services;

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
                Name = "Downtown Recycling Center",
                Address = "123 Main St",
                City = "Metropolis",
                State = "NY",
                ZipCode = "10001",
                DistanceInMiles = 0.5,
                PhoneNumber = "555-0101"
            },
            new RecyclingFacility
            {
                FacilityId = 2,
                Name = "Westside Drop-off",
                Address = "456 Oak Ave",
                City = "Metropolis",
                State = "NY",
                ZipCode = "10001",
                DistanceInMiles = 1.2,
                PhoneNumber = "555-0102"
            },
            new RecyclingFacility
            {
                FacilityId = 3,
                Name = "Eastside Materials Recovery",
                Address = "789 Pine Rd",
                City = "Gotham",
                State = "NJ",
                ZipCode = "07101",
                DistanceInMiles = 2.3,
                PhoneNumber = "555-0103"
            },
            new RecyclingFacility
            {
                FacilityId = 4,
                Name = "North Park Recycling",
                Address = "321 Elm Blvd",
                City = "Star City",
                State = "CA",
                ZipCode = "90210",
                DistanceInMiles = 3.0,
                PhoneNumber = "555-0104"
            },
            new RecyclingFacility
            {
                FacilityId = 5,
                Name = "South Bay Waste Solutions",
                Address = "654 Maple Dr",
                City = "Coast City",
                State = "CA",
                ZipCode = "90210",
                DistanceInMiles = 4.5,
                PhoneNumber = "555-0105"
            }
        };
    }

    public IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)
    {
        if (string.IsNullOrWhiteSpace(zipCode))
        {
            return Enumerable.Empty<RecyclingFacility>();
        }

        return _facilities
            .Where(f => f.ZipCode.Equals(zipCode.Trim(), StringComparison.OrdinalIgnoreCase))
            .OrderBy(f => f.DistanceInMiles)
            .ToList();
    }
}
```

// FILE: Controllers/RecyclingFacilitiesController.cs
```csharp
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
```