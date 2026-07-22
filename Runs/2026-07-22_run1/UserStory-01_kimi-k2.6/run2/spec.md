# Stage 1 Implementation Specification

## Architecture Overview
A minimal ASP.NET Core Web API (`net8.0`) using the standard controller-service pattern. The solution contains a single web project named `Implementation`. It exposes one GET endpoint `/api/recycling-facilities` that delegates to a synchronous, injectable service. The service operates exclusively on a candidate dataset supplied via its constructor, satisfying the white-box test requirements while keeping the live application compilable.

## Naming Contract Satisfaction

| Rule Requirement | Intended Implementation |
|------------------|-------------------------|
| **Project / Assembly / Root Namespace** | `Implementation.csproj`, assembly name `Implementation`, root namespace `Implementation`. |
| **Data Model** | `Implementation.Models.RecyclingFacility` with properties exactly matching the spec: `FacilityId` (int), `Name` (string), `Address` (string), `City` (string), `State` (string), `ZipCode` (string), `DistanceInMiles` (double), `PhoneNumber` (string). |
| **Service Interface** | `Implementation.Services.IGetNearbyRecyclingFacilitiesService` containing a single **synchronous** method: `IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode);` |
| **Service Implementation** | `Implementation.Services.GetNearbyRecyclingFacilitiesService` implementing the interface above. |
| **Service Constructor** | `public GetNearbyRecyclingFacilitiesService(IEnumerable<RecyclingFacility> facilities)`. The class stores this dataset in a private field and performs all lookups against it. No parameterless constructor and no hard-coded seed data inside the service. |
| **Parameter Validation** | The service method throws `System.ArgumentException` (or `ArgumentNullException`) when `zipCode` is `null`, `String.Empty`, or whitespace-only. |
| **Empty Results** | When no facilities match, the service returns an empty `IEnumerable<RecyclingFacility>` (never `null`). |
| **Controller / Route** | An ASP.NET Core controller mapped to `GET /api/recycling-facilities`. It reads the `zipCode` query parameter, invokes the service, and returns `200 OK` with the collection. |

## File List

| Relative Path | Purpose |
|---------------|---------|
| `Implementation.csproj` | SDK-style project file targeting `net8.0` and `Microsoft.NET.Sdk.Web`. |
| `Program.cs` | Application entry point. Configures the HTTP request pipeline and dependency injection container. |
| `Models/RecyclingFacility.cs` | Data model class in namespace `Implementation.Models`. |
| `Services/IGetNearbyRecyclingFacilitiesService.cs` | Service contract interface in namespace `Implementation.Services`. |
| `Services/GetNearbyRecyclingFacilitiesService.cs` | Service implementation in namespace `Implementation.Services`. |
| `Controllers/RecyclingFacilitiesController.cs` | API controller exposing `GET /api/recycling-facilities`. |
| `Data/SeedData.cs` *(optional)* | Static helper providing an in-memory `IEnumerable<RecyclingFacility>` solely for the live DI wiring. Not referenced by the service class internally and not used by tests. |

## Dependency Injection & Live Wiring

`Program.cs` will register dependencies so the live application compiles and the controller resolves its service:
1. Register an `IEnumerable<RecyclingFacility>` instance (or factory) containing seed data for the running app.
2. Register `IGetNearbyRecyclingFacilitiesService` → `GetNearbyRecyclingFacilitiesService`, allowing the DI container to inject the previously registered dataset into the service’s constructor.

Because the unit tests construct `GetNearbyRecyclingFacilitiesService` directly with their own dataset, the live seed data is never executed during testing; it exists only to satisfy the compile-time requirement that the controller’s dependencies can be resolved in a running application.

## Validation & Filtering Strategy

Inside `GetNearbyRecyclingFacilitiesService.GetNearbyRecyclingFacilities(string zipCode)`:
1. Validate `zipCode` using `string.IsNullOrWhiteSpace`. If invalid, throw `ArgumentException`.
2. Filter the injected `IEnumerable<RecyclingFacility>` where the facility’s `ZipCode` matches the supplied value.
3. Materialize and return the results as `IEnumerable<RecyclingFacility>`. If the filtered set is empty, return an empty collection.

## Endpoint Mapping

- **HTTP Method:** `GET`
- **Route:** `/api/recycling-facilities`
- **Query Parameter:** `zipCode` (string, required)
- **Success Response:** `200 OK` with a JSON array of `RecyclingFacility` objects.