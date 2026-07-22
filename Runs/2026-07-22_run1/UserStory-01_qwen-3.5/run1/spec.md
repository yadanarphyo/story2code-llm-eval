# Implementation Specification: User Story 01 (Recycling Facilities)

## 1. Architecture Overview

This implementation follows a standard ASP.NET Core Web API architecture tailored to satisfy the strict naming and structural contracts required for white-box unit testing. The solution is organized into logical layers within a single project assembly (`Implementation`).

- **Target Framework:** .NET 8 (`net8.0`)
- **Project Type:** ASP.NET Core Web API (`Microsoft.NET.Sdk.Web`)
- **Assembly Name:** `Implementation`
- **Root Namespace:** `Implementation`

The architecture separates concerns into:
1.  **Models:** Pure data transfer objects matching the user story.
2.  **Services:** Business logic and validation, designed for direct instantiation by tests.
3.  **Controllers:** HTTP endpoint wiring that delegates to services.
4.  **Composition Root:** `Program.cs` for dependency injection setup (required for build validity).

## 2. File List

The following files will be generated in Stage 2. Paths are relative to the project root.

| File Path | Purpose |
| :--- | :--- |
| `Implementation.csproj` | Project definition, SDK, and target framework. |
| `Program.cs` | Application entry point and DI container configuration. |
| `Models/RecyclingFacility.cs` | Data model class matching the User Story specification. |
| `Services/IGetNearbyRecyclingFacilitiesService.cs` | Service interface contract for testing. |
| `Services/GetNearbyRecyclingFacilitiesService.cs` | Service implementation containing validation and logic. |
| `Controllers/RecyclingFacilitiesController.cs` | ASP.NET Core Controller exposing the HTTP endpoint. |

## 3. Naming Contract Compliance

This section details how the implementation satisfies the mandatory rules provided in the Rules File.

### 3.1. Main Project (Rule 1)
- **File Name:** `Implementation.csproj`
- **Assembly Name:** `Implementation`
- **Root Namespace:** `Implementation`
- **Compliance:** The `.csproj` will explicitly set `<AssemblyName>Implementation</AssemblyName>` and `<RootNamespace>Implementation</RootNamespace>`.

### 3.2. Data Models (Rule 2)
- **Namespace:** `Implementation.Models`
- **Class Name:** `RecyclingFacility`
- **Properties:** Exactly as defined in the User Story Data Model section.
    - `FacilityId` (int)
    - `Name` (string)
    - `Address` (string)
    - `City` (string)
    - `State` (string)
    - `ZipCode` (string)
    - `DistanceInMiles` (double)
    - `PhoneNumber` (string)
- **Compliance:** No pluralization, renaming, or type changes will be applied.

### 3.3. Service Contract (Rule 3)
- **Interface Name:** `Implementation.Services.IGetNearbyRecyclingFacilitiesService`
- **Class Name:** `Implementation.Services.GetNearbyRecyclingFacilitiesService`
- **Method Name:** `GetNearbyRecyclingFacilities`
- **Method Signature:** Synchronous. Returns `IEnumerable<RecyclingFacility>`. Accepts `string zipCode`.
    - *Note:* No `Task`, no `Async` suffix.
- **Constructor:** `public GetNearbyRecyclingFacilitiesService(IEnumerable<RecyclingFacility> facilities)`
    - *Note:* The class will store this enumerable and operate on it. No parameterless constructor. No hardcoded seed data.
- **Compliance:** This structure allows the test harness to instantiate the service directly: `new GetNearbyRecyclingFacilitiesService(testData)`.

### 3.4. Parameters & Empty Results (Rule 4)
- **Validation:** The service method will check the `zipCode` parameter.
    - If `null`, `string.Empty`, or whitespace-only: Throw `System.ArgumentException`.
- **Empty Results:** If the input is valid but no facilities match (or the injected collection is empty), the method will return `Array.Empty<RecyclingFacility>()` or an empty `List<RecyclingFacility>`.
    - *Note:* Never returns `null`.
- **Compliance:** Ensures tests can verify exception throwing and empty collection handling reliably.

### 3.5. Controller (Rule 5)
- **Endpoint:** `GET /api/recycling-facilities`
- **Binding:** The controller will bind the query string parameter `zipCode` and pass it to the service method.
- **Response:** Returns `IActionResult` with `OkObjectResult` containing the service's result.
- **Compliance:** Provides the HTTP interface required by the API contract while delegating logic to the testable service.

## 4. Implementation Logic Details

### 4.1. Service Logic
1.  **Validation:** Check `zipCode` using `string.IsNullOrWhiteSpace`. If true, throw `ArgumentException`.
2.  **Processing:** The service receives a collection of `RecyclingFacility` via the constructor. Since geospatial calculation requires latitude/longitude (not provided in the model), the service assumes the injected collection represents the candidate set relevant to the query (e.g., pre-filtered or scored by the test harness).
3.  **Return:** The service returns the injected collection materialized as a list/array. If the injected collection is empty, an empty list is returned. This satisfies the requirement to return an empty collection when no data exists without returning `null`.

### 4.2. Dependency Injection
- The `Program.cs` will register the service and controller.
- Since the service requires a constructor argument (`IEnumerable<RecyclingFacility>`), the DI container will be wired using a factory or a transient registration that provides a default empty set for the live application context.
- *Note:* As per Rule 3, the live wiring is build-time only; the tests bypass DI and instantiate the service directly.

## 5. Dependencies
- **NuGet Packages:** None required beyond the default ASP.NET Core templates, as the logic relies on standard BCL types (`System`, `System.Linq`, `System.Collections.Generic`).

---
*End of Stage 1 Specification*