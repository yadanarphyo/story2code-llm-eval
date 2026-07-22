# Stage 1 Implementation Specification — User Story 01

## Architecture Overview

The solution is a minimal ASP.NET Core Web API (`net8.0`) using the standard Controller → Service pattern.  
No persistence, repository, or DTO layers are required because the service operates directly on an injected in-memory dataset (`IEnumerable<RecyclingFacility>`). The controller translates the HTTP GET request into a call to the synchronous service method and returns the resulting JSON array with an HTTP 200 OK status.

---

## File List

| Relative Path | Purpose |
|---------------|---------|
| `Implementation.csproj` | SDK-style Web project targeting `net8.0`; references the ASP.NET Core Web SDK. |
| `Program.cs` | Application bootstrap, routing, and DI container wiring. |
| `Models/RecyclingFacility.cs` | Data model class with the exact properties specified in the user story. |
| `Services/IGetNearbyRecyclingFacilitiesService.cs` | Service contract interface. |
| `Services/GetNearbyRecyclingFacilitiesService.cs` | Service implementation containing search/filter logic and parameter validation. |
| `Controllers/RecyclingFacilitiesController.cs` | ASP.NET Core API controller exposing `GET /api/recycling-facilities`. |

---

## Naming Contract Compliance

### Project
- **Assembly name / root namespace:** `Implementation`  
- **Project file:** `Implementation.csproj`

### Data Model
- **Namespace:** `Implementation.Models`
- **Class:** `RecyclingFacility`
- **Properties (exact names and types):**
  - `int FacilityId`
  - `string Name`
  - `string Address`
  - `string City`
  - `string State`
  - `string ZipCode`
  - `double DistanceInMiles`
  - `string PhoneNumber`

### Service Contract
- **Interface:** `Implementation.Services.IGetNearbyRecyclingFacilitiesService`
- **Method signature:**  
  `IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)`
  - The method is **synchronous** (return type is not wrapped in `Task<>`).
- **Implementation class:** `Implementation.Services.GetNearbyRecyclingFacilitiesService`
  - Implements `IGetNearbyRecyclingFacilitiesService`.
  - Constructor: `public GetNearbyRecyclingFacilitiesService(IEnumerable<RecyclingFacility> facilities)`
  - Contains **no parameterless constructor** and **no hard-coded or fallback seed data**.
  - Operates exclusively on the `IEnumerable<RecyclingFacility>` supplied via constructor injection.

### Controller
- An ASP.NET Core controller (name is unconstrained) will handle:
  - **HTTP Method:** `GET`
  - **Route:** `/api/recycling-facilities`
  - **Query parameter:** `zipCode`
- It delegates to `IGetNearbyRecyclingFacilitiesService.GetNearbyRecyclingFacilities` and returns the collection as an HTTP 200 OK response.

---

## Behavior & Validation Rules

1. **Required Parameter Validation**  
   The service method inspects the `zipCode` argument. If it is `null`, `string.Empty`, or consists solely of whitespace characters, the method throws an `ArgumentException` (or a subclass such as `ArgumentNullException`).

2. **Search Logic**  
   On valid input, the service filters the injected dataset for records whose `ZipCode` property exactly matches the supplied `zipCode` value (string equality). The matching entries are returned as an `IEnumerable<RecyclingFacility>`.

3. **Empty Results**  
   When no records match the supplied zip code, the service returns an empty collection (e.g., `Enumerable.Empty<RecyclingFacility>()`). It **never** returns `null` and **never** throws.

---

## Dependency Injection Strategy

Because `GetNearbyRecyclingFacilitiesService` requires `IEnumerable<RecyclingFacility>` in its constructor, `Program.cs` will register a singleton/factory for the dataset (for example, a static in-memory list of seed facilities) and then register the service implementation so the DI container can satisfy its constructor.  
This ensures the project compiles and can run as a live application, while the service class remains fully decoupled from any specific data source and testable with externally supplied datasets.