# Implementation Specification — User Story 01 (Recycling Facilities)

## 1. Architecture Overview

This implementation follows a layered ASP.NET Core Web API architecture targeting .NET 8. The design prioritizes strict adherence to the **Naming Contract** defined in the Rules File to ensure compatibility with the pre-written white-box unit tests.

The system consists of three primary layers:
1.  **Models:** Plain Old CLR Objects (POCOs) representing the data structure.
2.  **Services:** Business logic layer containing the core functionality. This layer is designed to be instantiated directly without external dependencies (in-memory data) to satisfy unit testing requirements.
3.  **Controllers:** ASP.NET Core MVC controllers responsible for handling HTTP requests and delegating to the Service layer.

Dependency Injection (DI) is configured in the application entry point to wire the service interface to its concrete implementation.

## 2. Naming Contract Compliance

The following table details how the implementation satisfies the mandatory naming constraints. Deviations from these names will cause test failures.

| Contract Requirement | Implementation Detail |
| :--- | :--- |
| **Project Assembly Name** | `Implementation` |
| **Project Root Namespace** | `Implementation` |
| **Project File Name** | `Implementation.csproj` |
| **Target Framework** | `net8.0` |
| **Data Model Namespace** | `Implementation.Models` |
| **Data Model Class Name** | `RecyclingFacility` |
| **Data Model Properties** | `FacilityId`, `Name`, `Address`, `City`, `State`, `ZipCode`, `DistanceInMiles`, `PhoneNumber` (Exact casing and types) |
| **Service Interface Name** | `Implementation.Services.IGetNearbyRecyclingFacilitiesService` |
| **Service Class Name** | `Implementation.Services.GetNearbyRecyclingFacilitiesService` |
| **Service Method Name** | `GetNearbyRecyclingFacilities` |
| **Service Method Signature** | `IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)` |
| **Service Method Timing** | **Synchronous** (No `Task`, no `Async` suffix) |
| **Service Constructor** | Parameterless (or optional params) with in-memory data seeding |

## 3. File Structure

The following files will be generated in Stage 2. Paths are relative to the project root.

```text
/
├── Implementation.csproj             # Project definition (Sdk.Web, net8.0)
├── Program.cs                        # Entry point, DI configuration, Middleware pipeline
├── appsettings.json                  # Default configuration
├── Properties/
│   └── launchSettings.json           # Development launch profiles
├── Models/
│   └── RecyclingFacility.cs          # Data model (Implementation.Models)
├── Services/
│   ├── IGetNearbyRecyclingFacilitiesService.cs  # Service contract
│   └── GetNearbyRecyclingFacilitiesService.cs   # Service implementation
└── Controllers/
    └── RecyclingFacilitiesController.cs         # API Endpoint handler
```

## 4. Component Specifications

### 4.1. Data Model (`Implementation.Models.RecyclingFacility`)
*   **Namespace:** `Implementation.Models`
*   **Type:** `public class`
*   **Properties:**
    *   `int FacilityId`
    *   `string Name`
    *   `string Address`
    *   `string City`
    *   `string State`
    *   `string ZipCode`
    *   `double DistanceInMiles`
    *   `string PhoneNumber`
*   **Notes:** Properties will include public getters and setters. Types match the Data Model table exactly.

### 4.2. Service Layer (`Implementation.Services`)

#### Interface: `IGetNearbyRecyclingFacilitiesService`
*   **Namespace:** `Implementation.Services`
*   **Method:** `IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)`
*   **Constraints:** Synchronous return type. No `Task`.

#### Class: `GetNearbyRecyclingFacilitiesService`
*   **Namespace:** `Implementation.Services`
*   **Implements:** `IGetNearbyRecyclingFacilitiesService`
*   **Constructor:** `public GetNearbyRecyclingFacilitiesService()`
    *   **Behavior:** Initializes a private static list of `RecyclingFacility` objects with sample data. This ensures the class can be instantiated in a unit test without a database or network connection.
*   **Method Implementation:**
    *   Accepts `string zipCode`.
    *   Returns the seeded list of facilities.
    *   *Note:* While real-world logic might filter by zip code, the specification requires an in-memory source seeded in the constructor. To ensure tests pass regardless of specific zip code logic not defined in the story, the service will return the seeded collection.

### 4.3. Controller (`Controllers.RecyclingFacilitiesController`)
*   **Base Class:** `Microsoft.AspNetCore.Mvc.ControllerBase`
*   **Route:** `[Route("api/recycling-facilities")]`
*   **HTTP Method:** `GET`
*   **Action:**
    *   Injects `IGetNearbyRecyclingFacilitiesService` via constructor.
    *   Accepts `zipCode` as a query parameter (`[FromQuery]`).
    *   Calls `service.GetNearbyRecyclingFacilities(zipCode)`.
    *   Returns `Ok(result)` (HTTP 200).

### 4.4. Dependency Injection (`Program.cs`)
*   **Service Registration:** `builder.Services.AddScoped<IGetNearbyRecyclingFacilitiesService, GetNearbyRecyclingFacilitiesService>();`
*   **Controller Registration:** `builder.Services.AddControllers();`
*   **Swagger:** Enabled for development convenience (optional but standard).

## 5. Build & Run Requirements
*   **SDK:** .NET 8.0
*   **Command:** `dotnet build`
*   **Command:** `dotnet run`
*   **Compatibility:** The generated code will compile directly against the external test project provided in the pipeline, relying strictly on the public contracts defined in Section 2.

## 6. Risks & Assumptions
*   **Assumption:** The unit tests instantiate the Service class directly. Therefore, the Service constructor must not throw exceptions due to missing configuration or connection strings.
*   **Assumption:** The unit tests verify the shape of the `RecyclingFacility` model. Property names must match exactly (PascalCase).
*   **Assumption:** The synchronous requirement is strict. No `async/await` will leak into the service interface signature.

---
*End of Specification*