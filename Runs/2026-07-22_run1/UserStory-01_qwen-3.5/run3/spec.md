# Implementation Specification: User Story 01 (Recycling Facilities)

## 1. Overview
This document specifies the architecture and design for the **Implementation** project targeting **User Story 01**. The solution is an ASP.NET Core Web API built on **.NET 8**. It adheres strictly to the **Story2Code Rules File** regarding naming contracts, service signatures, and data model definitions to ensure compatibility with the pre-written unit test harness.

## 2. Project Structure
The project will follow a standard layered ASP.NET Core structure. All files reside under the root namespace `Implementation`.

| File Path | Description |
| :--- | :--- |
| `Implementation.csproj` | Project file targeting `net8.0`, `Microsoft.NET.Sdk.Web`. |
| `Program.cs` | Application entry point, DI configuration, and middleware pipeline. |
| `Models/RecyclingFacility.cs` | Data model class matching the User Story Data Model. |
| `Services/IGetNearbyRecyclingFacilitiesService.cs` | Service interface contract. |
| `Services/GetNearbyRecyclingFacilitiesService.cs` | Service implementation containing business logic. |
| `Controllers/RecyclingFacilitiesController.cs` | API Controller exposing the HTTP endpoint. |

## 3. Naming Contract Compliance
The following table maps the Rules File requirements to the specific identifiers used in this implementation.

| Rule Requirement | Implementation Detail |
| :--- | :--- |
| **Assembly/Root Namespace** | `Implementation` (defined in `.csproj` and `Program.cs`) |
| **Data Model Namespace** | `Implementation.Models` |
| **Data Model Class** | `RecyclingFacility` (Exact property names/types per User Story) |
| **Service Interface** | `Implementation.Services.IGetNearbyRecyclingFacilitiesService` |
| **Service Class** | `Implementation.Services.GetNearbyRecyclingFacilitiesService` |
| **Service Method** | `IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)` |
| **Service Constructor** | `GetNearbyRecyclingFacilitiesService(IEnumerable<RecyclingFacility> facilities)` |
| **Method Synchronicity** | Synchronous (No `Task`, no `Async` suffix) |
| **Required Param Validation** | Throws `ArgumentException` for null/empty/whitespace `zipCode` |
| **Empty Result Handling** | Returns empty `IEnumerable<RecyclingFacility>` (never `null`) |

## 4. Component Design

### 4.1. Data Model (`Implementation.Models.RecyclingFacility`)
This class will be a plain C# object (POCO) with public getters and setters. It will strictly match the properties defined in the User Story Data Model section:
- `FacilityId` (int)
- `Name` (string)
- `Address` (string)
- `City` (string)
- `State` (string)
- `ZipCode` (string)
- `DistanceInMiles` (double)
- `PhoneNumber` (string)

### 4.2. Service Layer (`Implementation.Services`)
#### Interface: `IGetNearbyRecyclingFacilitiesService`
- Defines a single method: `IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode)`.
- Located in namespace `Implementation.Services`.

#### Class: `GetNearbyRecyclingFacilitiesService`
- Implements `IGetNearbyRecyclingFacilitiesService`.
- **Constructor:** Accepts `IEnumerable<RecyclingFacility> facilities`. This dependency is injected by the test harness during unit testing.
- **Logic:**
    1.  **Validation:** Checks the `zipCode` parameter. If `null`, empty, or whitespace, throws `System.ArgumentException`.
    2.  **Filtering:** Filters the injected `facilities` collection. The implementation will match facilities where the `ZipCode` property equals the input `zipCode`. (Note: As `DistanceInMiles` is provided in the model, the test data is expected to contain pre-calculated distances relevant to the query; the service focuses on selecting the relevant subset based on the input parameter).
    3.  **Return:** Returns the filtered collection. If no matches are found, returns an empty list (e.g., via `.ToList()` on an empty LINQ result), never `null`.

### 4.3. Controller (`Controllers.RecyclingFacilitiesController`)
- **Route:** `[route("/api/recycling-facilities")]` or equivalent attribute routing.
- **Method:** `HttpGet`.
- **Logic:**
    1.  Injects `IGetNearbyRecyclingFacilitiesService` via constructor.
    2.  Extracts `zipCode` from the Query String.
    3.  Calls the service method.
    4.  Returns `Ok(result)` (HTTP 200).
- **Naming:** The controller class name is unconstrained by the rules; `RecyclingFacilitiesController` will be used for clarity.

### 4.4. Dependency Injection (`Program.cs`)
- The application must compile and run independently of the test harness.
- **Registration:** `GetNearbyRecyclingFacilitiesService` will be registered as a transient service.
- **Data Dependency:** Since the service constructor requires `IEnumerable<RecyclingFacility>`, the DI container must be configured to provide this. For the live application (non-test) context, a factory will be registered to provide an empty list or a static seed list of dummy data. This satisfies the compilation requirement without interfering with the test harness, which instantiates the service class directly with specific test data.

## 5. Validation & Error Handling Strategy
- **Required Parameters:** The `zipCode` parameter is marked `Required: Yes`.
- **Implementation:** The service method `GetNearbyRecyclingFacilities` will perform the validation logic immediately upon entry.
- **Exception Type:** `System.ArgumentException` (or `ArgumentNullException`) will be thrown for invalid inputs. This aligns with Rule 4.
- **HTTP Mapping:** The Controller will not catch this specific exception explicitly for this spec; default ASP.NET Core behavior will map unhandled exceptions to 500, but since the tests call the service directly (white-box), the exception throwing is the primary verification point. If the controller is invoked via HTTP, standard API behavior applies.

## 6. Build & Runtime Requirements
- **SDK:** .NET 8.0
- **Type:** `Microsoft.NET.Sdk.Web`
- **NuGet Packages:** No external packages are strictly required beyond the default ASP.NET Core templates, though `System.Linq` is used implicitly.
- **Compatibility:** The code will be structured to ensure the test project (provided externally) can compile against the `Implementation` assembly, specifically referencing the `Services` and `Models` namespaces.

## 7. Next Steps
Upon approval of this specification, Stage 2 will generate the full source code for all files listed in Section 2, adhering strictly to the naming contracts and logic described above.