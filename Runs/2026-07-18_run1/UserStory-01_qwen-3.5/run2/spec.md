# Implementation Specification — User Story 01 (Recycling Facilities)

## 1. Architecture Overview

This implementation follows a layered ASP.NET Core Web API architecture designed for testability and strict adherence to the provided naming contract. The solution is structured to allow the pre-written unit tests to instantiate the service layer directly without external dependencies (database, network, etc.).

### Layers
1.  **Presentation Layer:** ASP.NET Core Controllers responsible for handling HTTP requests, model binding, and returning JSON responses.
2.  **Service Layer:** Contains the business logic interface and implementation. This layer is designed to be instantiated directly by tests using in-memory data.
3.  **Domain/Model Layer:** Contains the POCO data models that match the user story's data model exactly.

### Dependency Injection
The `Program.cs` entry point will configure the Dependency Injection (DI) container to register the service implementation against its interface. This allows the controller to depend on the abstraction while ensuring the concrete implementation uses in-memory data suitable for testing.

---

## 2. Naming Contract Compliance

The following table details how the generated code will satisfy the mandatory naming rules defined in the Rules File.

| Rule Requirement | Specification Detail |
| :--- | :--- |
| **Assembly Name** | `Implementation` |
| **Root Namespace** | `Implementation` |
| **Target Framework** | `net8.0` |
| **SDK** | `Microsoft.NET.Sdk.Web` |
| **Data Model Namespace** | `Implementation.Models` |
| **Data Model Class** | `RecyclingFacility` |
| **Data Model Properties** | `FacilityId`, `Name`, `Address`, `City`, `State`, `ZipCode`, `DistanceInMiles`, `PhoneNumber` (Exact types and casing) |
| **Service Namespace** | `Implementation.Services` |
| **Service Interface** | `IGetNearbyRecyclingFacilitiesService` |
| **Service Class** | `GetNearbyRecyclingFacilitiesService` |
| **Service Method** | `GetNearbyRecyclingFacilities` (Synchronous, no `Task`, no `Async` suffix) |
| **Service Method Signature** | `IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)` |
| **Service Constructor** | Parameterless (or optional params) seeding in-memory data for testability |
| **Controller** | Unconstrained name (e.g., `RecyclingFacilitiesController`), wired to `GET /api/recycling-facilities` |

---

## 3. File Manifest

The project will consist of the following files located in the project root.

| File Path | Description |
| :--- | :--- |
| `Implementation.csproj` | Project file defining SDK, framework, and dependencies. |
| `Program.cs` | Application entry point, DI configuration, and middleware pipeline. |
| `appsettings.json` | Default configuration file (standard ASP.NET Core template). |
| `Models/RecyclingFacility.cs` | Defines the `RecyclingFacility` class in `Implementation.Models`. |
| `Services/IGetNearbyRecyclingFacilitiesService.cs` | Defines the service contract in `Implementation.Services`. |
| `Services/GetNearbyRecyclingFacilitiesService.cs` | Implements the service logic with in-memory data in `Implementation.Services`. |
| `Controllers/RecyclingFacilitiesController.cs` | ASP.NET Core controller exposing the HTTP endpoint. |

---

## 4. Component Specifications

### 4.1. Project File (`Implementation.csproj`)
*   **SDK:** `Microsoft.NET.Sdk.Web`
*   **Target Framework:** `net8.0`
*   **Assembly Name:** `Implementation`
*   **Root Namespace:** `Implementation`
*   **Dependencies:** Standard ASP.NET Core packages required for building a Web API (e.g., `Microsoft.AspNetCore.OpenApi` if needed for Swagger, though not strictly required for tests).

### 4.2. Data Model (`Models/RecyclingFacility.cs`)
*   **Namespace:** `Implementation.Models`
*   **Class Name:** `RecyclingFacility`
*   **Properties:**
    *   `int FacilityId`
    *   `string Name`
    *   `string Address`
    *   `string City`
    *   `string State`
    *   `string ZipCode`
    *   `double DistanceInMiles`
    *   `string PhoneNumber`
*   **Notes:** Properties will use automatic getters and setters. No data annotations are strictly required unless needed for serialization, but default JSON serialization behavior will be relied upon.

### 4.3. Service Layer (`Services/`)

#### Interface (`IGetNearbyRecyclingFacilitiesService.cs`)
*   **Namespace:** `Implementation.Services`
*   **Interface Name:** `IGetNearbyRecyclingFacilitiesService`
*   **Method:**
    *   Name: `GetNearbyRecyclingFacilities`
    *   Parameters: `string zipCode`
    *   Return Type: `IEnumerable<RecyclingFacility>`
    *   Modifiers: Public (explicit interface implementation not required, but method must be accessible).
    *   **Constraint:** Synchronous. No `Task`, no `async` keyword on signature.

#### Implementation (`GetNearbyRecyclingFacilitiesService.cs`)
*   **Namespace:** `Implementation.Services`
*   **Class Name:** `GetNearbyRecyclingFacilitiesService`
*   **Implements:** `IGetNearbyRecyclingFacilitiesService`
*   **Constructor:** Public parameterless constructor.
    *   **Behavior:** Initializes a private static list of `RecyclingFacility` objects with seed data. This ensures the class can be instantiated in a unit test without external services.
*   **Method Implementation:**
    *   Executes the logic to filter/return facilities based on the provided `zipCode`.
    *   Since this is a spec for a testable unit, the logic may simulate distance calculation or return pre-seeded data that matches the query pattern expected by tests.
    *   Returns the resolved `IEnumerable<RecyclingFacility>` directly.

### 4.4. Controller Layer (`Controllers/RecyclingFacilitiesController.cs`)
*   **Namespace:** `Implementation.Controllers` (or root `Implementation` depending on folder structure, but recommended `Controllers` sub-namespace).
*   **Base Class:** `ControllerBase`
*   **Route Attribute:** `[Route("api/recycling-facilities")]`
*   **Method:**
    *   HTTP Verb: `GET`
    *   Route: `""` (Empty string to match base route)
    *   Parameter: `string zipCode` (bound from Query String via `[FromQuery]`).
    *   Logic: Injects `IGetNearbyRecyclingFacilitiesService`, calls `GetNearbyRecyclingFacilities(zipCode)`, and returns `Ok(result)`.

### 4.5. Entry Point (`Program.cs`)
*   **Builder:** Creates `WebApplication.CreateBuilder(args)`.
*   **Services:**
    *   Adds Controllers (`AddControllers()`).
    *   Registers Service: `builder.Services.AddScoped<IGetNearbyRecyclingFacilitiesService, GetNearbyRecyclingFacilitiesService>();` (Or Transient/Singleton; Scoped is standard for web requests).
*   **Pipeline:**
    *   Maps Controllers (`MapControllers()`).
    *   Standard middleware (Authorization, Routing, etc.).

---

## 5. Testing Strategy Note

To satisfy the rule regarding unit test instantiation:
*   The `GetNearbyRecyclingFacilitiesService` will **not** rely on `IOptions`, `DbContext`, `HttpClient`, or any external injected dependencies in its constructor.
*   All data required to satisfy a query will be hardcoded within the service constructor or defined as static readonly fields initialized at startup.
*   This ensures the test suite can run `new GetNearbyRecyclingFacilitiesService()` without configuring a DI container or mocking external resources.

---

## 6. Next Steps

Upon approval of this specification, Stage 2 will generate the full source code for every file listed in the File Manifest, enclosed in fenced code blocks with the required `// FILE: <path>` headers, ready for compilation against the pre-written test suite.