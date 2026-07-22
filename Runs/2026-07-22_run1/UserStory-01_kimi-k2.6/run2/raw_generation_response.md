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
using Implementation.Data;
using Implementation.Models;
using Implementation.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register seed data for the live application
builder.Services.AddSingleton<IEnumerable<RecyclingFacility>>(SeedData.GetFacilities());

// Register the service
builder.Services.AddSingleton<IGetNearbyRecyclingFacilitiesService, GetNearbyRecyclingFacilitiesService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthorization();
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

        return _facilities.Where(f => f.ZipCode == zipCode).ToList();
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
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    [HttpGet]
    public ActionResult<IEnumerable<RecyclingFacility>> GetNearbyRecyclingFacilities([FromQuery] string zipCode)
    {
        var result = _service.GetNearbyRecyclingFacilities(zipCode);
        return Ok(result);
    }
}
```

// FILE: Data/SeedData.cs
```csharp
using Implementation.Models;

namespace Implementation.Data;

public static class SeedData
{
    public static IEnumerable<RecyclingFacility> GetFacilities()
    {
        return new List<RecyclingFacility>
        {
            new RecyclingFacility
            {
                FacilityId = 1,
                Name = "Downtown Recycling Center",
                Address = "123 Main St",
                City = "Springfield",
                State = "IL",
                ZipCode = "62701",
                DistanceInMiles = 1.2,
                PhoneNumber = "555-0101"
            },
            new RecyclingFacility
            {
                FacilityId = 2,
                Name = "Westside Materials Recovery",
                Address = "456 Oak Ave",
                City = "Springfield",
                State = "IL",
                ZipCode = "62704",
                DistanceInMiles = 3.5,
                PhoneNumber = "555-0102"
            }
        };
    }
}
```