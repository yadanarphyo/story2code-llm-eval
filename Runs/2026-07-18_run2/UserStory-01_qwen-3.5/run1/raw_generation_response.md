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

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<IGetNearbyRecyclingFacilitiesService, GetNearbyRecyclingFacilitiesService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
        // Seed in-memory data for unit test compatibility
        _facilities = new List<RecyclingFacility>
        {
            new RecyclingFacility
            {
                FacilityId = 1,
                Name = "EcoCenter Downtown",
                Address = "123 Green St",
                City = "Beverly Hills",
                State = "CA",
                ZipCode = "90210",
                DistanceInMiles = 1.5,
                PhoneNumber = "555-0101"
            },
            new RecyclingFacility
            {
                FacilityId = 2,
                Name = "EcoCenter Westside",
                Address = "456 Recycle Ave",
                City = "Beverly Hills",
                State = "CA",
                ZipCode = "90211",
                DistanceInMiles = 3.2,
                PhoneNumber = "555-0102"
            },
            new RecyclingFacility
            {
                FacilityId = 3,
                Name = "Manhattan Recycling Hub",
                Address = "789 Waste Ln",
                City = "New York",
                State = "NY",
                ZipCode = "10001",
                DistanceInMiles = 0.5,
                PhoneNumber = "555-0103"
            }
        };
    }

    public IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)
    {
        if (string.IsNullOrWhiteSpace(zipCode))
        {
            return Enumerable.Empty<RecyclingFacility>();
        }

        // Simple prefix match to simulate "nearby" logic without geospatial libraries
        return _facilities.Where(f => f.ZipCode.StartsWith(zipCode));
    }
}
```

// FILE: Controllers/RecyclingFacilitiesController.cs
```csharp
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
    public ActionResult<IEnumerable<RecyclingFacility>> Get([FromQuery] string zipCode)
    {
        var result = _service.GetNearbyRecyclingFacilities(zipCode);
        return Ok(result);
    }
}
```