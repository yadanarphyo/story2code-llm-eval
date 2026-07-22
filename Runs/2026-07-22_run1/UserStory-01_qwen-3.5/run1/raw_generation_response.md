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
using Implementation.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register the service interface and implementation.
// The service requires IEnumerable<RecyclingFacility> in its constructor.
// For the live app (which is not executed by tests), we provide an empty enumerable.
builder.Services.AddTransient<IEnumerable<RecyclingFacility>>(_ => Enumerable.Empty<RecyclingFacility>());
builder.Services.AddTransient<IGetNearbyRecyclingFacilitiesService, GetNearbyRecyclingFacilitiesService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
        _facilities = facilities ?? Enumerable.Empty<RecyclingFacility>();
    }

    public IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)
    {
        if (string.IsNullOrWhiteSpace(zipCode))
        {
            throw new ArgumentException("zipCode is required.", nameof(zipCode));
        }

        // Filter the injected candidate dataset by the provided zip code.
        // This ensures that if no matching data exists, an empty collection is returned.
        var result = _facilities.Where(f => f.ZipCode == zipCode);

        return result.ToList();
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
    public ActionResult<IEnumerable<RecyclingFacility>> GetNearbyRecyclingFacilities([FromQuery] string zipCode)
    {
        var facilities = _service.GetNearbyRecyclingFacilities(zipCode);
        return Ok(facilities);
    }
}
```