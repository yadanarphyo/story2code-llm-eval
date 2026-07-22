# Stage 1 Implementation Specification — User Story 01

## 1. Architecture Overview

The solution is a minimal, self-contained ASP.NET Core Web API targeting **.NET 8** (`net8.0`). It uses a single-tier service layer backed by an **in-memory seeded collection** so that the core business logic can be instantiated and exercised by unit tests without any external database, network call, or configuration file.

### Design Choices
- **No persistence layer** (EF Core / SQL / external API) is required. The service implementation holds a static/private list of `RecyclingFacility` objects initialized inside its parameterless constructor.
- **Synchronous service contract** per the naming rules: the service method returns the raw `IEnumerable<RecyclingFacility>` shape directly, not wrapped in `Task<T>`.
- **Standard ASP.NET Core DI**: the service interface is registered in `Program.cs` and injected into the controller.
- **Lightweight validation**: the service returns an empty collection (or a filtered subset) for unrecognized / empty `zipCode` values rather than throwing, keeping the controller action simple and predictable for white-box tests.

---

## 2. Naming Contract Compliance

| Rule Requirement | How It Is Satisfied |
|------------------|---------------------|
| **Project / Assembly / Root Namespace** | The project file is named `Implementation.csproj`. `<AssemblyName>` and `<RootNamespace>` are both set to `Implementation`. |
| **Data Model** | `RecyclingFacility` is defined in namespace `Implementation.Models` with the exact property names and CLR types specified in the user story (`FacilityId`, `Name`, `Address`, `City`, `State`, `ZipCode`, `DistanceInMiles`, `PhoneNumber`). |
| **Service Interface** | `Implementation.Services.IGetNearbyRecyclingFacilitiesService` declares the synchronous method: `IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode);` |
| **Service Implementation** | `Implementation.Services.GetNearbyRecyclingFacilitiesService` implements the interface above. Its constructor is parameterless and seeds an in-memory list of facilities. |
| **Controller** | An ASP.NET Core controller (name unconstrained) exposes `GET /api/recycling-facilities`, reads the `zipCode` query string, delegates to `IGetNearbyRecyclingFacilitiesService`, and returns `200 OK` with the JSON array. |

---

## 3. File List & Responsibilities

```
Implementation.csproj
Program.cs
Models/
  RecyclingFacility.cs
Services/
  IGetNearbyRecyclingFacilitiesService.cs
  GetNearbyRecyclingFacilitiesService.cs
Controllers/
  RecyclingFacilitiesController.cs
```

| File | Responsibility |
|------|----------------|
| `Implementation.csproj` | SDK-style Web project (`Microsoft.NET.Sdk.Web`), targeting `net8.0`, with default implicit usings enabled. |
| `Program.cs` | Bootstraps the Web API builder, registers `IGetNearbyRecyclingFacilitiesService` → `GetNearbyRecyclingFacilitiesService` in the DI container, and maps controllers. |
| `Models/RecyclingFacility.cs` | Defines the `RecyclingFacility` POCO in the `Implementation.Models` namespace. |
| `Services/IGetNearbyRecyclingFacilitiesService.cs` | Defines the service contract interface in the `Implementation.Services` namespace. |
| `Services/GetNearbyRecyclingFacilitiesService.cs` | Implements the contract. Contains a private `List<RecyclingFacility>` seeded in the parameterless constructor. The `GetNearbyRecyclingFacilities(string zipCode)` method filters / sorts this list by matching zip code proximity and returns the results synchronously. |
| `Controllers/RecyclingFacilitiesController.cs` | Declares a controller with a route prefix of `api/recycling-facilities`. The `Get` action binds the required `zipCode` query parameter, invokes the injected service, and returns an `OkObjectResult`. |

---

## 4. Request Flow

1. **HTTP Request** → `GET /api/recycling-facilities?zipCode=62701`
2. **Controller** → `RecyclingFacilitiesController` extracts `zipCode` from the query string.
3. **Service Call** → Invokes `IGetNearbyRecyclingFacilitiesService.GetNearbyRecyclingFacilities(zipCode)`.
4. **In-Memory Query** → `GetNearbyRecyclingFacilitiesService` scans its seeded list, filters facilities relevant to the supplied zip code, and computes/returns `DistanceInMiles`.
5. **HTTP Response** → Controller wraps the `IEnumerable<RecyclingFacility>` in `200 OK`; ASP.NET Core serializes it to the JSON array shape documented in the user story.

---

## 5. Testability Strategy

Because the pre-written unit tests compile directly against the implementation assembly and instantiate the service class:
- `GetNearbyRecyclingFacilitiesService` exposes a **public parameterless constructor** that pre-populates a private in-memory store with a diverse set of `RecyclingFacility` instances.
- No `IConfiguration`, `HttpClient`, or database context is required as a constructor argument.
- The service is therefore fully unit-testable in isolation: tests can construct it directly, call `GetNearbyRecyclingFacilities`, and assert on the returned `IEnumerable<RecyclingFacility>` without mocking external infrastructure.