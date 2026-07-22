# Stage 1 Implementation Specification — User Story 01

## Architecture Overview

The solution follows a minimal layered architecture within a single ASP.NET Core Web API project:

- **Models** — A single POCO representing the recycling facility.
- **Services** — A synchronous, in-memory service that fulfills the lookup contract. The service is seeded with hard-coded sample data in its constructor so that unit tests can instantiate it directly without external dependencies.
- **Controllers** — A standard ASP.NET Core API controller that exposes the required HTTP endpoint, delegates to the service, and returns the JSON array via `Ok(...)`.

No external database, HTTP client, or geocoding service is required. Dependency Injection will wire the service interface to its implementation at startup.

## File List

| Relative Path | Purpose |
|---------------|---------|
| `Implementation.csproj` | SDK-style project file (`Microsoft.NET.Sdk.Web`, `net8.0`, assembly name / root namespace `Implementation`). |
| `Models/RecyclingFacility.cs` | Data model class with the exact property names and types specified. |
| `Services/IGetNearbyRecyclingFacilitiesService.cs` | Service interface defining the synchronous contract method. |
| `Services/GetNearbyRecyclingFacilitiesService.cs` | Service implementation backed by an in-memory list seeded in the parameterless constructor. |
| `Controllers/RecyclingFacilitiesController.cs` | Controller handling `GET /api/recycling-facilities` and delegating to the service. |
| `Program.cs` | Standard bootstrap registering the service in DI and mapping controller routes. |

## Naming Contract Compliance

The following pinned names and shapes are used exactly as required:

1. **Project / Assembly / Root Namespace:** `Implementation`
2. **Data Model:** `Implementation.Models.RecyclingFacility` containing:
   - `int FacilityId`
   - `string Name`
   - `string Address`
   - `string City`
   - `string State`
   - `string ZipCode`
   - `double DistanceInMiles`
   - `string PhoneNumber`
3. **Service Interface:** `Implementation.Services.IGetNearbyRecyclingFacilitiesService` with the synchronous method:
   - `IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)`
4. **Service Implementation:** `Implementation.Services.GetNearbyRecyclingFacilitiesService` implementing the interface above. It provides a parameterless constructor that initializes an in-memory collection of sample facilities, satisfying the requirement that tests can instantiate it without live external resources.
5. **Controller:** Wires the service to respond to `GET /api/recycling-facilities?zipCode={zipCode}` and returns an `HTTP 200 OK` payload matching the documented JSON shape.

## Design Notes

- **Synchronous Service:** Although ASP.NET Core is async-by-default, the service method remains synchronous (`IEnumerable<RecyclingFacility>` rather than `Task<...>`) per the naming contract. Any internal filtering is performed synchronously over the in-memory list.
- **In-Memory Seeding:** `GetNearbyRecyclingFacilitiesService` will contain a private list of `RecyclingFacility` objects initialized directly in its constructor. The `GetNearbyRecyclingFacilities(string zipCode)` implementation will filter and project from this list based on the supplied `zipCode`.
- **DI Registration:** `Program.cs` will register `IGetNearbyRecyclingFacilitiesService` with `GetNearbyRecyclingFacilitiesService` using standard `AddScoped` (or `AddSingleton`) so the controller can receive it via constructor injection.