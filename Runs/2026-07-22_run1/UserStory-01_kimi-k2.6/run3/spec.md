# Stage 1 Implementation Specification — User Story 01

## 1. Architecture Overview

The solution is a standard **ASP.NET Core Web API** (`net8.0`) using a minimal two-layer controller-service architecture.  
Because the service operates entirely on an in-memory dataset injected at construction time, no persistence layer (EF Core, repository, etc.) is required. The design prioritizes satisfying the strict naming contract while keeping the live application compilable and runnable.

- **Presentation Layer:** A single ASP.NET Core controller exposing `GET /api/recycling-facilities`.
- **Service Layer:** A synchronous service that filters / returns `RecyclingFacility` instances based on the supplied `zipCode`.
- **Data Layer:** The service consumes an `IEnumerable<RecyclingFacility>` provided via constructor injection. The live app wires a seeded list through DI; the unit tests instantiate the service directly with their own dataset.

## 2. File List

| Relative Path | Purpose |
|---------------|---------|
| `Implementation.csproj` | Project file targeting `net8.0`, SDK `Microsoft.NET.Sdk.Web`. Assembly name and root namespace set to `Implementation`. |
| `Program.cs` | Application bootstrap. Registers `IEnumerable<RecyclingFacility>` (seeded for the live app) and `IGetNearbyRecyclingFacilitiesService` → `GetNearbyRecyclingFacilitiesService` in the DI container. Maps controllers. |
| `Models/RecyclingFacility.cs` | Data model class in namespace `Implementation.Models`. |
| `Services/IGetNearbyRecyclingFacilitiesService.cs` | Service contract interface in namespace `Implementation.Services`. |
| `Services/GetNearbyRecyclingFacilitiesService.cs` | Service implementation in namespace `Implementation.Services`. |
| `Controllers/RecyclingFacilitiesController.cs` | API controller (name is unconstrained) decorated with `[ApiController]` and `[Route("api/recycling-facilities")]`. Accepts `zipCode` query string, delegates to service, and returns `200 OK`. |

## 3. Naming Contract Compliance

The following items are pinned exactly as required by the rules and will appear verbatim in the implementation:

- **Root namespace / assembly:** `Implementation`
- **Data model namespace:** `Implementation.Models`
  - Class name: `RecyclingFacility`
  - Properties (case-sensitive):
    - `int FacilityId`
    - `string Name`
    - `string Address`
    - `string City`
    - `string State`
    - `string ZipCode`
    - `double DistanceInMiles`
    - `string PhoneNumber`
- **Service namespace:** `Implementation.Services`
  - Interface: `IGetNearbyRecyclingFacilitiesService`
  - Implementation: `GetNearbyRecyclingFacilitiesService`
  - Method signature (synchronous, no `Async` suffix, no `Task<T>` wrapper):
    ```csharp
    IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)
    ```
  - Constructor signature (single parameter, no parameterless constructor):
    ```csharp
    public GetNearbyRecyclingFacilitiesService(IEnumerable<RecyclingFacility> facilities)
    ```
- **Required parameter validation:** Because `zipCode` is marked **Required: Yes**, the service method will throw `System.ArgumentException` (or `ArgumentNullException`) when the argument is `null`, `string.Empty`, or whitespace-only. For valid inputs that yield no matches, it returns an empty `IEnumerable<RecyclingFacility>` (never `null`).
- **Controller route:** `GET /api/recycling-facilities` wired to invoke the service and return the collection with HTTP 200.

## 4. Dependency Injection & Live-App Wiring

Although the pre-written tests construct `GetNearbyRecyclingFacilitiesService` directly, the live application must still compile. `Program.cs` will:

1. Register an `IEnumerable<RecyclingFacility>` singleton containing a small seed dataset (or an empty list) so the container can satisfy the service constructor.
2. Register `IGetNearbyRecyclingFacilitiesService` with its implementation so the controller can resolve it.

This satisfies the build-time requirement without affecting the white-box tests, which bypass DI entirely.