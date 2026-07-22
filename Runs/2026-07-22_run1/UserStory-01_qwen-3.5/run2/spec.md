# Implementation Specification: User Story 01 (Recycling Facilities)

## 1. Architecture Overview

The solution will be implemented as a standard **ASP.NET Core Web API** targeting **.NET 8**. The architecture follows a layered approach to separate concerns while strictly adhering to the naming contracts required for white-box testing.

-   **Entry Point:** `Program.cs` configures the Web API host, dependency injection container, and middleware pipeline.
-   **Controllers:** ASP.NET Core Controllers handle HTTP requests, model binding, and response formatting. They delegate business logic to the Service layer.
-   **Services:** Contains the core business logic. This layer is designed specifically to satisfy the testing contract (synchronous methods, constructor injection of data).
-   **Models:** Contains the POCO data classes representing the domain entities as defined in the User Story.

## 2. Project Structure & File List

The project root will be named `Implementation`. All files will reside within this structure.

| Relative Path | File Name | Description |
| :--- | :--- | :--- |
| `/` | `Implementation.csproj` | Project file targeting `net8.0`, `Microsoft.NET.Sdk.Web`. |
| `/` | `Program.cs` | Application entry point, DI configuration, and middleware setup. |
| `/` | `appsettings.json` | Default configuration file (required by template). |
| `/Models/` | `RecyclingFacility.cs` | Data model class matching the User Story schema. |
| `/Services/` | `IGetNearbyRecyclingFacilitiesService.cs` | Service interface contract. |
| `/Services/` | `GetNearbyRecyclingFacilitiesService.cs` | Service implementation containing logic and validation. |
| `/Controllers/` | `RecyclingFacilitiesController.cs` | API Controller exposing the GET endpoint. |

## 3. Naming Contract Compliance

This section details how the implementation satisfies the mandatory naming and structural rules defined in the Rules File.

### 3.1 Project and Namespace
-   **Assembly Name:** `Implementation`
-   **Root Namespace:** `Implementation`
-   **Project File:** `Implementation.csproj`
-   **Target Framework:** `net8.0`

### 3.2 Data Models
-   **Namespace:** `Implementation.Models`
-   **Class Name:** `RecyclingFacility`
-   **Properties:** Exact match to User Story (case-sensitive):
    -   `int FacilityId`
    -   `string Name`
    -   `string Address`
    -   `string City`
    -   `string State`
    -   `string ZipCode`
    -   `double DistanceInMiles`
    -   `string PhoneNumber`

### 3.3 Service Contract
-   **Interface Name:** `Implementation.Services.IGetNearbyRecyclingFacilitiesService`
-   **Class Name:** `Implementation.Services.GetNearbyRecyclingFacilitiesService`
-   **Method Name:** `GetNearbyRecyclingFacilities`
-   **Method Signature:** `IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)`
    -   **Synchronous:** Returns `IEnumerable<RecyclingFacility>` directly (no `Task`).
    -   **Parameters:** Single `string zipCode`.
-   **Constructor:** `public GetNearbyRecyclingFacilitiesService(IEnumerable<RecyclingFacility> facilities)`
    -   Injects the dataset externally. No internal seeding.
    -   No parameterless constructor.

### 3.4 Parameter Validation & Return Rules
-   **Required Parameter:** `zipCode`
    -   **Validation:** The service method will check if `zipCode` is `null`, empty, or whitespace using `string.IsNullOrWhiteSpace`.
    -   **Exception:** Throws `System.ArgumentException` (or `ArgumentNullException`) if validation fails.
-   **Empty Results:** If no facilities match the criteria, the method returns an empty collection (e.g., `Array.Empty<RecyclingFacility>()`), never `null`.

## 4. Implementation Details

### 4.1 Service Logic (`GetNearbyRecyclingFacilitiesService`)
1.  **Constructor:** Stores the injected `IEnumerable<RecyclingFacility>` in a private readonly field.
2.  **Method Execution:**
    -   Validate `zipCode`. Throw `ArgumentException` if invalid.
    -   Query the stored collection.
    -   **Filtering Logic:** Since geospatial calculation requires latitude/longitude not present in the model, the service will filter the injected collection where the `RecyclingFacility.ZipCode` matches the input `zipCode`. This ensures deterministic behavior based on the provided data model.
    -   Return the filtered results as `IEnumerable<RecyclingFacility>`.

### 4.2 Controller (`RecyclingFacilitiesController`)
-   **Route:** `[Route("api/recycling-facilities")]`
-   **Method:** `HttpGet`
-   **Parameter Binding:** Binds `zipCode` from the Query String (`[FromQuery]`).
-   **Execution:** Instantiates or injects the service (via DI for live app, direct call compatible for tests) and returns the result.
    -   *Note:* To satisfy the test requirement where tests instantiate the service directly (`new GetNearbyRecyclingFacilitiesService(testData)`), the Controller will rely on DI for the live app, but the Service class itself remains decoupled from ASP.NET types to allow direct instantiation by the test harness.

### 4.3 Dependency Injection (`Program.cs`)
-   The Service interface and implementation will be registered in the DI container.
-   **Registration Strategy:** `builder.Services.AddTransient<IGetNearbyRecyclingFacilitiesService, GetNearbyRecyclingFacilitiesService>();`
-   **Data Supply:** Since the Service requires data in its constructor, a factory pattern or a seeded singleton list will be registered to provide the `IEnumerable<RecyclingFacility>` dependency when the app runs live. This ensures the application compiles and runs, though the pipeline tests will bypass this wiring by constructing the service directly.

## 5. Compliance Checklist

-   [ ] Project name is `Implementation`.
-   [ ] Models are in `Implementation.Models`.
-   [ ] Service Interface is `IGetNearbyRecyclingFacilitiesService`.
-   [ ] Service Class is `GetNearbyRecyclingFacilitiesService`.
-   [ ] Service Method is synchronous (`IEnumerable<...>`).
-   [ ] Service Constructor accepts `IEnumerable<RecyclingFacility>`.
-   [ ] Required parameter `zipCode` throws `ArgumentException` on invalid input.
-   [ ] Empty results return empty collection, not null.
-   [ ] No hardcoded data in Service class.
-   [ ] .NET 8 Web API SDK.