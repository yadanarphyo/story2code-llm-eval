# Implementation Specification: User Story 01 (Recycling Facilities)

## 1. Overview
This specification outlines the architecture and file structure for the **Implementation** project, designed to satisfy User Story 01. The solution is an ASP.NET Core Web API targeting .NET 8 (`net8.0`). It adheres strictly to the **Story2Code Rules File** regarding naming contracts, namespace structure, and service synchronization requirements to ensure compatibility with the pre-written white-box unit tests.

## 2. Architecture Design
The solution follows a layered architecture simplified for testability and compliance with the naming contract.

1.  **Presentation Layer (Controller):** An ASP.NET Core Controller will handle HTTP requests. It will be routed to `GET /api/recycling-facilities`. It accepts the `zipCode` query parameter and delegates logic to the service layer.
2.  **Service Layer:** A dedicated service class implementing the required interface. This layer contains the business logic for filtering/recycling facilities.
    *   **Constraint:** The service method must be **synchronous** (`IEnumerable<T>`, not `Task<T>`) to match the test harness expectations.
    *   **Data Source:** To satisfy the requirement that the service must be instantiable without external dependencies (DB/Network), the service will utilize an **in-memory data store** seeded within its constructor.
3.  **Domain Layer (Models):** Data transfer objects representing the `RecyclingFacility` entity, placed in the required namespace.

## 3. Naming Contract Compliance
The following table maps the Rules File requirements to the specific implementation details for this story.

| Rule Requirement | Implementation Detail |
| :--- | :--- |
| **Project Name** | `Implementation.csproj` |
| **Root Namespace** | `Implementation` |
| **Assembly Name** | `Implementation` |
| **Model Namespace** | `Implementation.Models` |
| **Model Class Name** | `RecyclingFacility` |
| **Model Properties** | `FacilityId`, `Name`, `Address`, `City`, `State`, `ZipCode`, `DistanceInMiles`, `PhoneNumber` (Exact PascalCase match) |
| **Service Interface** | `Implementation.Services.IGetNearbyRecyclingFacilitiesService` |
| **Service Class** | `Implementation.Services.GetNearbyRecyclingFacilitiesService` |
| **Service Method** | `IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)` |
| **Service Method Type** | **Synchronous** (No `Task`, no `Async` suffix) |
| **Service Constructor** | Parameterless (or optional params) using in-memory data for testability |
| **HTTP Endpoint** | `GET /api/recycling-facilities` |

## 4. Data Model Specification
**Namespace:** `Implementation.Models`
**Class:** `public class RecyclingFacility`

| Property | Type | Notes |
| :--- | :--- | :--- |
| `FacilityId` | `int` | Unique identifier |
| `Name` | `string` | Facility name |
| `Address` | `string` | Street address |
| `City` | `string` | City name |
| `State` | `string` | State/Region code |
| `ZipCode` | `string` | Postal code |
| `DistanceInMiles` | `double` | Calculated distance |
| `PhoneNumber` | `string` | Contact number |

*Note: ASP.NET Core default JSON serialization will convert these PascalCase properties to camelCase in the HTTP response, matching the API Interface JSON example.*

## 5. Service Layer Specification
**Namespace:** `Implementation.Services`

### Interface: `IGetNearbyRecyclingFacilitiesService`
```csharp
public interface IGetNearbyRecyclingFacilitiesService
{
    IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode);
}
```

### Class: `GetNearbyRecyclingFacilitiesService`
- Implements `IGetNearbyRecyclingFacilitiesService`.
- **Constructor:** Public parameterless constructor. Initializes a private `List<RecyclingFacility>` with dummy data to ensure tests can run without external infrastructure.
- **Method:** `GetNearbyRecyclingFacilities(string zipCode)`
    - Logic: Filters the in-memory list based on the provided `zipCode`.
    - Return: `IEnumerable<RecyclingFacility>`.

## 6. Controller Specification
**Namespace:** `Implementation.Controllers` (Flexible per rules)
**Class Name:** `RecyclingFacilitiesController` (Flexible per rules)
**Base Class:** `ControllerBase`

- **Route:** `[Route("api/recycling-facilities")]`
- **Method:** `GET`
- **Action:** `GetNearbyRecyclingFacilities`
- **Parameter:** `[FromQuery] string zipCode`
- **Dependency:** Injects `IGetNearbyRecyclingFacilitiesService` via constructor.
- **Response:** Returns `Ok(result)` where result is the IEnumerable from the service.

## 7. File List (Stage 2 Generation Plan)
The following files will be generated in Stage 2 to build the complete project:

1.  `Implementation.csproj`
    *   SDK: `Microsoft.NET.Sdk.Web`
    *   TargetFramework: `net8.0`
2.  `Program.cs`
    *   Entry point.
    *   Configures Dependency Injection (registers Service interface/class).
    *   Configures Middleware (Controllers, etc.).
3.  `Models/RecyclingFacility.cs`
    *   Data model definition.
4.  `Services/IGetNearbyRecyclingFacilitiesService.cs`
    *   Service contract interface.
5.  `Services/GetNearbyRecyclingFacilitiesService.cs`
    *   Service implementation with in-memory data.
6.  `Controllers/RecyclingFacilitiesController.cs`
    *   API Endpoint wiring.

## 8. Dependencies
- **Framework:** .NET 8.0
- **NuGet Packages:** None required beyond default ASP.NET Core templates (using built-in `System.Text.Json` and DI containers).

## 9. Risk Mitigation
- **Synchronous Service Requirement:** The rules explicitly forbid `async/Task` on the service method interface. Internal implementation details (if any) must resolve asynchronously before returning, but given the in-memory requirement, purely synchronous logic will be used to avoid complexity and deadlocks.
- **Testability:** The service constructor will not accept required dependencies that cannot be mocked easily without a DI container. It will be self-contained to allow `new GetNearbyRecyclingFacilitiesService()` in unit tests.

---
*End of Specification*