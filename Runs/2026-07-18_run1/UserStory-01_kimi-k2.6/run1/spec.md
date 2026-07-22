# Stage 1 Implementation Specification — User Story 01

## Architecture Overview

The solution is a standard ASP.NET Core Web API using the traditional MVC controller pipeline. It is intentionally thin and testable:

- **Controller layer** — A single `RecyclingFacilitiesController` handles the HTTP GET request at `/api/recycling-facilities`, extracts the `zipCode` query parameter, delegates to the service, and returns an `OK` result.
- **Service layer** — `GetNearbyRecyclingFacilitiesService` encapsulates all data retrieval and business logic. Because the project must be unit-testable without a live database or network call, the service seeds an in-memory collection of `RecyclingFacility` objects inside its parameterless constructor. The service method is **synchronous**, returning `IEnumerable<RecyclingFacility>` directly.
- **Model layer** — A plain POCO, `RecyclingFacility`, lives in the required namespace with the exact property names and types specified in the data model section.

No external database, HTTP client, or file I/O is required. Dependency injection wires the service interface to its implementation at startup.

---

## File List

| Relative Path | Purpose |
|---------------|---------|
| `Implementation.csproj` | .NET 8 Web SDK project file. `<AssemblyName>` and `<RootNamespace>` are both `Implementation`. |
| `Program.cs` | Application entry point. Adds controllers, registers `IGetNearbyRecyclingFacilitiesService` → `GetNearbyRecyclingFacilitiesService` in DI, and builds the pipeline. |
| `Models/RecyclingFacility.cs` | Data model class in namespace `Implementation.Models`. Contains the exact properties: `FacilityId`, `Name`, `Address`, `City`, `State`, `ZipCode`, `DistanceInMiles`, `PhoneNumber`. |
| `Services/IGetNearbyRecyclingFacilitiesService.cs` | Service contract interface in namespace `Implementation.Services`. Declares the synchronous method: `IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode);`. |
| `Services/GetNearbyRecyclingFacilitiesService.cs` | Service implementation in namespace `Implementation.Services`. Implements the interface above. Uses a parameterless constructor that seeds a static in-memory list of facilities. The `GetNearbyRecyclingFacilities` method filters/returns matching results (e.g., exact zip match or seeded demo data) as a plain enumerable. |
| `Controllers/RecyclingFacilitiesController.cs` | ASP.NET Core API controller. Decorated with `[Route("api/recycling-facilities")]` and `[ApiController]`. The GET action accepts `string zipCode` from the query string, calls the injected service, and returns `Ok(result)`. |

---

## Naming Contract Compliance

| Requirement | How It Is Satisfied |
|-------------|---------------------|
| **Project / namespace** | Project file is `Implementation.csproj`; `<AssemblyName>` and `<RootNamespace>` are exactly `Implementation`. |
| **Data model** | `Implementation.Models.RecyclingFacility` is defined with the exact property names (`FacilityId`, `Name`, `Address`, `City`, `State`, `ZipCode`, `DistanceInMiles`, `PhoneNumber`) and types (`int`, `string`, `string`, `string`, `string`, `string`, `double`, `string`). |
| **Service interface** | `Implementation.Services.IGetNearbyRecyclingFacilitiesService` declares a single synchronous method: `IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode);`. |
| **Service implementation** | `Implementation.Services.GetNearbyRecyclingFacilitiesService` implements the interface. Its parameterless constructor seeds an in-memory data source, allowing direct instantiation in unit tests without external dependencies. |
| **Synchronous method** | The method is not suffixed with `Async` and does not return `Task<T>`. Any internal async work (if ever needed) would be awaited inside the method body before returning the final `IEnumerable<RecyclingFacility>`. |
| **Controller / route** | A controller exposes `GET /api/recycling-facilities` and passes the `zipCode` query string to the service method above. |