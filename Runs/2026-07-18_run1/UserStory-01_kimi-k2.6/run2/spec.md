# Stage 1 Implementation Specification

## Architecture Overview

The solution is a minimal, self-contained **ASP.NET Core Web API** targeting **.NET 8** (`net8.0`). It is structured into three standard layers—**Models**, **Services**, and **Controllers**—with no external database, file system, or network dependencies.  

The `GetNearbyRecyclingFacilitiesService` maintains an **in-memory collection** of `RecyclingFacility` objects, seeded directly inside its parameterless constructor. This guarantees that the pre-written unit tests can instantiate the service with `new GetNearbyRecyclingFacilitiesService()` and receive deterministic results without mocking infrastructure.  

Dependency injection wiring in `Program.cs` simply registers the service interface with its concrete implementation. The controller is a standard API controller that accepts the `zipCode` query parameter, invokes the service synchronously, and returns the result as an HTTP 200 OK response.

---

## File List

| Relative Path | Purpose |
|---------------|---------|
| `Implementation.csproj` | Project file (`Microsoft.NET.Sdk.Web`, `net8.0`). |
| `Program.cs` | Application bootstrap and DI registration. |
| `Models/RecyclingFacility.cs` | Data model class with the exact property names and types from the user story. |
| `Services/IGetNearbyRecyclingFacilitiesService.cs` | Service interface defining the synchronous contract method. |
| `Services/GetNearbyRecyclingFacilitiesService.cs` | Service implementation backed by an in-memory seeded list. |
| `Controllers/RecyclingFacilitiesController.cs` | API controller exposing `GET /api/recycling-facilities`. |

---

## Naming Contract Compliance

- **Main Project**  
  The project file is named `Implementation.csproj`. Both `<AssemblyName>` and `<RootNamespace>` are set to exactly `Implementation`.

- **Data Model**  
  The class `RecyclingFacility` lives in namespace `Implementation.Models` and exposes the following public properties with the exact names and types specified:  
  `FacilityId` (int), `Name` (string), `Address` (string), `City` (string), `State` (string), `ZipCode` (string), `DistanceInMiles` (double), `PhoneNumber` (string).

- **Service Contract**  
  - Interface: `Implementation.Services.IGetNearbyRecyclingFacilitiesService`  
  - Method signature (synchronous, non-async): `IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)`  
  - Implementation: `Implementation.Services.GetNearbyRecyclingFacilitiesService` implementing the interface above.  
  - Testability: The parameterless constructor seeds a static in-memory list of facilities, satisfying the requirement that the class can be instantiated directly in a unit test without external services.

- **Controller & Route**  
  An ASP.NET Core API controller is wired to handle `GET` requests at `/api/recycling-facilities`. It reads the required `zipCode` query string, delegates to `IGetNearbyRecyclingFacilitiesService.GetNearbyRecyclingFacilities`, and returns the resulting `IEnumerable<RecyclingFacility>` as JSON with HTTP status 200.