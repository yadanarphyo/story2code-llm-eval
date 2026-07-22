# Implementation Specification — User Story 01 (Recycling Facilities)

## 1. Architecture Overview

This specification outlines the implementation of the **GetNearbyRecyclingFacilities** feature within an ASP.NET Core Web API targeting .NET 8. The architecture prioritizes strict adherence to the **Story2Code Naming Contract** to ensure compatibility with the pre-written white-box unit tests.

The solution follows a layered approach:
1.  **Models:** Plain Old CLR Objects (POCOs) representing the data contract.
2.  **Services:** Business logic layer containing the synchronous method required by the test harness. This layer will utilize an in-memory data store seeded at construction to satisfy the "no external dependencies" constraint for unit testing.
3.  **Controllers:** ASP.NET Core controllers responsible for handling HTTP requests, model binding, and delegating to the service layer.
4.  **Composition Root:** `Program.cs` will handle Dependency Injection (DI) registration.

## 2. Naming Contract Compliance

The following table verifies how the implementation will satisfy the mandatory naming rules provided in the Rules File.

| Rule Requirement | Implementation Detail |
| :--- | :--- |
| **Project Assembly/Namespace** | **`Implementation`** (File: `Implementation.csproj`, Root Namespace: `Implementation`) |
| **Data Model Namespace** | `Implementation.Models` |
| **Data Model Class** | `RecyclingFacility` |
| **Data Model Properties** | `FacilityId`, `Name`, `Address`, `City`, `State`, `ZipCode`, `DistanceInMiles`, `PhoneNumber` (Exact case & types) |
| **Service Interface** | `Implementation.Services.IGetNearbyRecyclingFacilitiesService` |
| **Service Class** | `Implementation.Services.GetNearbyRecyclingFacilitiesService` |
| **Service Method** | `IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)` |
| **Method Signature** | **Synchronous**. No `Task`, no `Async` suffix. |
| **Service Constructor** | Parameterless (or optional params). Seeds in-memory data for testability. |
| **HTTP Endpoint** | `GET /api/recycling-facilities` |
| **Query Parameter** | `zipCode` (string) |

## 3. File Manifest

The following files will be generated in Stage 2. All paths are relative to the solution root.

| File Path | Description |
| :--- | :--- |
| `Implementation.csproj` | Project file targeting `net8.0`, `Microsoft.NET.Sdk.Web`. |
| `Program.cs` | Entry point. Configures DI, registers services, and maps controllers. |
| `appsettings.json` | Default configuration file. |
| `appsettings.Development.json` | Development-specific configuration. |
| `Properties/launchSettings.json` | IIS/Kestrel launch profiles. |
| `Models/RecyclingFacility.cs` | Defines the `RecyclingFacility` class in `Implementation.Models`. |
| `Services/IGetNearbyRecyclingFacilitiesService.cs` | Interface definition in `Implementation.Services`. |
| `Services/GetNearbyRecyclingFacilitiesService.cs` | Concrete implementation in `Implementation.Services`. |
| `Controllers/RecyclingFacilitiesController.cs` | ASP.NET Core controller exposing the API endpoint. |

## 4. Implementation Details

### 4.1 Data Model (`Implementation.Models.RecyclingFacility`)
The class will be defined with public getters and setters. Types will match the Data Model section exactly (e.g., `int` for `FacilityId`, `double` for `DistanceInMiles`, `string` for others).

### 4.2 Service Layer (`Implementation.Services`)
*   **Interface:** `IGetNearbyRecyclingFacilitiesService` will declare the single method `GetNearbyRecyclingFacilities`.
*   **Class:** `GetNearbyRecyclingFacilitiesService` will implement the interface.
*   **Data Seeding:** The constructor will initialize a `List<RecyclingFacility>` with dummy data (e.g., 3-5 facilities) to ensure the method returns valid results during unit testing without requiring a database.
*   **Logic:** The method will accept the `zipCode` string. For the purpose of this story, it will return the seeded list (optionally filtered if the zipCode matches, but primarily ensuring the signature and return type are correct for the tests).

### 4.3 Controller Layer (`Implementation.Controllers`)
*   **Class Name:** `RecyclingFacilitiesController` (inherits from `ControllerBase`).
*   **Route:** `[Route("api/recycling-facilities")]`.
*   **Method:** `GET` endpoint.
*   **Parameter Binding:** Uses `[FromQuery] string zipCode` to bind the query string parameter.
*   **Dependency Injection:** The service will be injected via the controller constructor.
*   **Response:** Returns `Ok(IEnumerable<RecyclingFacility>)`.

### 4.4 Dependency Injection
In `Program.cs`:
*   `builder.Services.AddScoped<IGetNearbyRecyclingFacilitiesService, GetNearbyRecyclingFacilitiesService>();`
*   `builder.Services.AddControllers();`

## 5. Risk Mitigation
*   **Synchronous Requirement:** Special care will be taken to ensure the service method does not return `Task<T>`. Any internal async operations (none expected for in-memory data) will be awaited internally before returning the resolved `IEnumerable<T>`.
*   **Namespace Accuracy:** All files will declare `namespace Implementation...` explicitly to prevent root namespace drift.
*   **Property Case Sensitivity:** C# properties will use PascalCase exactly as defined in the Data Model table to ensure JSON serialization matches the expected camelCase output via default ASP.NET Core JSON policies (or explicit configuration if needed, though default .NET 8 camelCase is standard).

---
*End of Specification*