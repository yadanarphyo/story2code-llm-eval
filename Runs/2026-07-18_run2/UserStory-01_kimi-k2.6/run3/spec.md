# Stage 1 Implementation Specification — User Story 01

## Architecture Overview

The solution is a standard ASP.NET Core Web API targeting .NET 8. It uses a minimal layered architecture consisting of three pinned layers:

1. **Models** – A single POCO representing the recycling facility.
2. **Services** – An in-memory service implementation that satisfies the synchronous contract required by the pre-written tests.
3. **Controllers** – A single API controller that adapts the HTTP GET request to the service call.

No external persistence, network clients, or third-party NuGet packages are required. The service is self-seeded with hard-coded sample data inside its constructor, making it fully unit-testable without a live database or external dependencies.

---

## File List

| # | Relative Path | Purpose |
|---|---------------|---------|
| 1 | `Implementation.csproj` | Project file. Targets `net8.0`, uses `Microsoft.NET.Sdk.Web`, and pins `<AssemblyName>` and `<RootNamespace>` to `Implementation`. |
| 2 | `Program.cs` | Application bootstrap. Registers the service interface/class pair in the DI container and wires controller routing. |
| 3 | `Models/RecyclingFacility.cs` | Data model class in namespace `Implementation.Models`. |
| 4 | `Services/IGetNearbyRecyclingFacilitiesService.cs` | Service contract interface in namespace `Implementation.Services`. |
| 5 | `Services/GetNearbyRecyclingFacilitiesService.cs` | Service implementation in namespace `Implementation.Services`. Contains a parameterless constructor that seeds an in-memory list of facilities. |
| 6 | `Controllers/RecyclingFacilitiesController.cs` | ASP.NET Core controller exposing `GET /api/recycling-facilities`. Accepts `zipCode` from the query string, delegates to the service, and returns `200 OK` with the JSON array. |

---

## Naming Contract Satisfaction

| Rule Requirement | How It Is Satisfied |
|------------------|---------------------|
| **Main project name / assembly / root namespace** | The project is named `Implementation.csproj`. Both `<AssemblyName>` and `<RootNamespace>` are set to exactly `Implementation`. |
| **Data model** | `RecyclingFacility` is defined in namespace `Implementation.Models` with public properties named exactly: `FacilityId` (`int`), `Name` (`string`), `Address` (`string`), `City` (`string`), `State` (`string`), `ZipCode` (`string`), `DistanceInMiles` (`double`), `PhoneNumber` (`string`). |
| **Service interface** | `Implementation.Services.IGetNearbyRecyclingFacilitiesService` declares a single synchronous method: `IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode);`. |
| **Service implementation** | `Implementation.Services.GetNearbyRecyclingFacilitiesService` implements the interface above. Its constructor requires no external dependencies; it initializes an internal in-memory collection of `RecyclingFacility` objects directly. |
| **Synchronous method shape** | The service method returns `IEnumerable<RecyclingFacility>` directly (not `Task<...>`) and is named exactly `GetNearbyRecyclingFacilities` with a single parameter named `zipCode` of type `string`. |
| **Controller / HTTP wiring** | A controller class (name unconstrained) handles `GET /api/recycling-facilities`, reads the required `zipCode` query parameter, invokes `GetNearbyRecyclingFacilities`, and returns the result as an HTTP 200 JSON array. |

---

## Design Notes

- **In-memory data store:** `GetNearbyRecyclingFacilitiesService` will hold a private `List<RecyclingFacility>` seeded in its parameterless constructor. The `GetNearbyRecyclingFacilities(string zipCode)` method will filter or project against this list and return matching results. Because the tests instantiate the service directly, no repository abstraction or database context is needed.
- **DI registration:** In `Program.cs`, `IGetNearbyRecyclingFacilitiesService` will be registered with its implementation using standard ASP.NET Core DI (e.g., `builder.Services.AddSingleton<IGetNearbyRecyclingFacilitiesService, GetNearbyRecyclingFacilitiesService>()` or Scoped).
- **Controller design:** The controller will use constructor injection for `IGetNearbyRecyclingFacilitiesService` and a standard `[HttpGet]` action method with `[FromQuery] string zipCode`. The action will return `Ok(result)` where `result` is the `IEnumerable<RecyclingFacility>` returned by the service.