# Implementation Specification — User Story 01

## 1. Architecture Overview

This implementation targets **.NET 8** (`net8.0`) using the **ASP.NET Core Web API** workload. The architecture follows a layered approach suitable for white-box unit testing, prioritizing the strict naming contracts required by the test pipeline while maintaining clean separation of concerns.

### High-Level Design
1.  **Entry Point:** Standard ASP.NET Core `Program.cs` configuring HTTP request pipeline and Dependency Injection (DI).
2.  **Controllers:** ASP.NET Core Controllers handle HTTP ingress, model binding, and response serialization.
3.  **Services:** Business logic resides in service classes. The specific service required by the test contract will contain in-memory data to ensure testability without external dependencies.
4.  **Models:** POCO classes representing the data structure, strictly adhering to the provided Data Model section.

### Dependency Injection
The service interface `IGetNearbyRecyclingFacilitiesService` and its implementation `GetNearbyRecyclingFacilitiesService` will be registered in the DI container (Scoped or Singleton) within `Program.cs`. The controller will request the interface via constructor injection.

## 2. Naming Contract Compliance

The following table details how the generated code will satisfy the mandatory naming constraints defined in the Rules File.

| Contract Requirement | Specified Value | Implementation Plan |
| :--- | :--- | :--- |
| **Project Name** | `Implementation` | The `.csproj` file will be named `Implementation.csproj`. `<AssemblyName>` and `<RootNamespace>` will be set to `Implementation`. |
| **Target Framework** | `net8.0` | Project will target .NET 8. |
| **Data Model Namespace** | `Implementation.Models` | All model classes will reside in this namespace. |
| **Data Model Class** | `RecyclingFacility` | Class name will match exactly. Properties will match exact names and types (e.g., `FacilityId`, `DistanceInMiles`). |
| **Service Namespace** | `Implementation.Services` | All service interfaces and classes will reside in this namespace. |
| **Service Interface** | `IGetNearbyRecyclingFacilitiesService` | Derived from Method Name `GetNearbyRecyclingFacilities`. |
| **Service Class** | `GetNearbyRecyclingFacilitiesService` | Derived from Method Name `GetNearbyRecyclingFacilities`. |
| **Service Method** | `GetNearbyRecyclingFacilities` | Synchronous method. No `Async` suffix. No `Task<>` wrapper. |
| **Service Method Signature** | `IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)` | Matches API Parameters (`zipCode`) and Response shape. |
| **Service Constructor** | Parameterless / In-Memory | The service class will include a public parameterless constructor seeding dummy data for unit test compatibility. |
| **Controller** | Unconstrained | Will be named `RecyclingFacilitiesController` in `Implementation.Controllers`. |

## 3. File Manifest

The following files will be generated in Stage 2. Paths are relative to the project root.

| File Path | Description |
| :--- | :--- |
| `Implementation.csproj` | Project file targeting `net8.0`, SDK `Microsoft.NET.Sdk.Web`. |
| `Program.cs` | Application entry point, DI configuration, and middleware setup. |
| `Models/RecyclingFacility.cs` | Data model class in namespace `Implementation.Models`. |
| `Services/IGetNearbyRecyclingFacilitiesService.cs` | Service contract interface in namespace `Implementation.Services`. |
| `Services/GetNearbyRecyclingFacilitiesService.cs` | Service implementation with in-memory data in namespace `Implementation.Services`. |
| `Controllers/RecyclingFacilitiesController.cs` | ASP.NET Core controller exposing `GET /api/recycling-facilities`. |

## 4. Detailed Design Notes

### 4.1 Data Model (`RecyclingFacility`)
The class will be defined in `Implementation.Models`. It will contain auto-implemented properties matching the Data Model table exactly:
- `FacilityId` (int)
- `Name` (string)
- `Address` (string)
- `City` (string)
- `State` (string)
- `ZipCode` (string)
- `DistanceInMiles` (double)
- `PhoneNumber` (string)

### 4.2 Service Layer (`GetNearbyRecyclingFacilitiesService`)
- **Namespace:** `Implementation.Services`
- **Interface:** `IGetNearbyRecyclingFacilitiesService` will define the single method `IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)`.
- **Implementation:** `GetNearbyRecyclingFacilitiesService` will implement the interface.
- **Data Persistence:** To satisfy the rule regarding unit test instantiation without external services, the constructor will initialize a `List<RecyclingFacility>` with static dummy data.
- **Logic:** The `GetNearbyRecyclingFacilities` method will filter this internal list based on the provided `zipCode`. For the purpose of this specification, a simple containment check or prefix match will be implemented to simulate "nearby" logic without requiring geospatial libraries.

### 4.3 Controller Layer (`RecyclingFacilitiesController`)
- **Namespace:** `Implementation.Controllers`
- **Route:** `[Route("api/recycling-facilities")]`
- **Method:** `GET`
- **Action:** `Get` action method will accept `[FromQuery] string zipCode`.
- **Execution:** The controller will inject `IGetNearbyRecyclingFacilitiesService`, call the synchronous method, and return the result via `Ok(...)`.

### 4.4 Edge Cases & Validation
- If `zipCode` is null or empty, the service may return an empty list or the controller may return `BadRequest`. Given the strict test contract focuses on the service method signature, the service will handle nulls gracefully (returning empty enumerable) to ensure the synchronous contract remains robust during direct instantiation tests.

## 5. Next Steps
Upon approval of this specification, Stage 2 will generate the full source code for the listed files within fenced code blocks, adhering strictly to the paths and naming conventions outlined above.