# Implementation Specification: User Story 01 (Recycling Facilities)

## 1. Project Overview
This specification outlines the architecture and file structure for the **Implementation** project, targeting **.NET 8 ASP.NET Core Web API**. The primary goal is to expose an endpoint for retrieving nearby recycling facilities based on a zip code, while strictly adhering to the **Story2Code Naming Contract** to ensure compatibility with pre-written white-box unit tests.

## 2. Architecture & Design Patterns

### 2.1. Layered Architecture
The project will follow a simplified layered architecture suitable for the scope of the user story:
1.  **Presentation Layer:** ASP.NET Core Controller to handle HTTP requests/responses.
2.  **Service Layer:** Business logic interface and implementation. This layer is designed to be directly instantiable by unit tests without external dependencies.
3.  **Domain/Model Layer:** POCO classes representing the data structure.

### 2.2. Dependency Injection
The ASP.NET Core built-in IoC container will be used. The service interface will be registered with a **Scoped** or **Singleton** lifetime (Singleton preferred for in-memory seed data consistency) in `Program.cs`.

### 2.3. Data Persistence Strategy
Per the rules file, the service must not require a live database. The `GetNearbyRecyclingFacilitiesService` will initialize an **in-memory list** of `RecyclingFacility` objects within its constructor. The logic for "nearby" will be simulated by filtering this list based on the provided `zipCode` parameter (exact match strategy for deterministic testing without geocoding services).

## 3. Naming Contract Compliance

The following table verifies adherence to the mandatory naming constraints:

| Contract Requirement | Specification Detail | Status |
| :--- | :--- | :--- |
| **Assembly Name** | `Implementation` | ✅ |
| **Root Namespace** | `Implementation` | ✅ |
| **Project File** | `Implementation.csproj` | ✅ |
| **Target Framework** | `net8.0` | ✅ |
| **Model Namespace** | `Implementation.Models` | ✅ |
| **Model Class Name** | `RecyclingFacility` | ✅ |
| **Model Properties** | Exact match to Data Model (e.g., `FacilityId`, `DistanceInMiles`) | ✅ |
| **Service Interface** | `Implementation.Services.IGetNearbyRecyclingFacilitiesService` | ✅ |
| **Service Class** | `Implementation.Services.GetNearbyRecyclingFacilitiesService` | ✅ |
| **Service Method** | `GetNearbyRecyclingFacilities` (Synchronous) | ✅ |
| **Return Type** | `IEnumerable<RecyclingFacility>` (No `Task`, no `Async`) | ✅ |
| **Parameter** | `string zipCode` | ✅ |

## 4. File Structure

The project will be organized as follows:

```text
Implementation/
├── Implementation.csproj          # Project manifest (Sdk=Microsoft.NET.Sdk.Web)
├── Program.cs                     # Entry point, DI configuration, Middleware pipeline
├── appsettings.json               # Default configuration
├── appsettings.Development.json   # Development configuration
├── Controllers/
│   └── RecyclingFacilitiesController.cs  # Handles GET /api/recycling-facilities
├── Models/
│   └── RecyclingFacility.cs       # Data model definition
└── Services/
    ├── IGetNearbyRecyclingFacilitiesService.cs  # Service contract
    └── GetNearbyRecyclingFacilitiesService.cs   # Service implementation
```

## 5. Component Specifications

### 5.1. Data Model (`Implementation.Models.RecyclingFacility`)
*   **Namespace:** `Implementation.Models`
*   **Properties:**
    *   `int FacilityId`
    *   `string Name`
    *   `string Address`
    *   `string City`
    *   `string State`
    *   `string ZipCode`
    *   `double DistanceInMiles`
    *   `string PhoneNumber`
*   **Notes:** All properties will have public getters and setters to allow serialization and object initialization.

### 5.2. Service Contract (`Implementation.Services.IGetNearbyRecyclingFacilitiesService`)
*   **Namespace:** `Implementation.Services`
*   **Method Signature:**
    ```csharp
    IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode);
    ```
*   **Constraints:** Strictly synchronous. No `Task` wrapper.

### 5.3. Service Implementation (`Implementation.Services.GetNearbyRecyclingFacilitiesService`)
*   **Namespace:** `Implementation.Services`
*   **Constructor:** Parameterless. Will seed a private `List<RecyclingFacility>` with dummy data upon instantiation.
*   **Method Logic:**
    1.  Accept `string zipCode`.
    2.  Filter the internal seed list where `facility.ZipCode` matches the input `zipCode`.
    3.  Return the filtered `IEnumerable<RecyclingFacility>`.
*   **Testability:** Can be instantiated via `new GetNearbyRecyclingFacilitiesService()` in a unit test without DI container or external services.

### 5.4. Controller (`Implementation.Controllers.RecyclingFacilitiesController`)
*   **Base Class:** `ControllerBase`
*   **Route Attribute:** `[Route("api/recycling-facilities")]`
*   **HTTP Method:** `GET`
*   **Action Method:**
    *   Injects `IGetNearbyRecyclingFacilitiesService` via constructor.
    *   Accepts `zipCode` from Query String (`[FromQuery]`).
    *   Calls service method.
    *   Returns `Ok(result)`.
*   **Validation:** Returns `BadRequest` if `zipCode` is null or empty (as per "Required" parameter constraint).

### 5.5. Entry Point (`Program.cs`)
*   **Builder Setup:** Creates `WebApplicationBuilder`.
*   **Service Registration:**
    *   `builder.Services.AddScoped<IGetNearbyRecyclingFacilitiesService, GetNearbyRecyclingFacilitiesService>();`
    *   Adds Controllers support (`AddControllers()`).
*   **Pipeline:**
    *   Maps controllers (`MapControllers()`).
*   **Namespace:** `Implementation` (or global using setup).

## 6. NuGet Dependencies
*   `Microsoft.NET.Sdk.Web` (Built-in)
*   No external NuGet packages are required for this specific user story implementation as it relies on in-memory data and standard ASP.NET Core features.

## 7. Next Steps (Stage 2)
Upon approval of this specification, Stage 2 will generate the full source code for each file listed in Section 4, enclosed in fenced code blocks with the required `// FILE: <path>` headers, ensuring exact compliance with the naming contract defined herein.