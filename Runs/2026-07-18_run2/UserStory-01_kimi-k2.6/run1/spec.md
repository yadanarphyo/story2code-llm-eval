# Stage 1 Implementation Specification — User Story 01

## Architecture Overview

The solution is a single ASP.NET Core Web API project using a conventional controller-service-model layout.  
Because the service must be instantiable in a unit test without external dependencies, the implementation uses an **in-memory seeded data source** inside the service class itself. No database, HTTP client, or file I/O is required at runtime or test time.

### Layers
1. **Models** – Plain C# classes representing the contract-defined data shape.  
2. **Services** – Business logic and “persistence” (in-memory list). The service is synchronous as required.  
3. **Controllers** – Thin HTTP adapter that validates the query parameter, delegates to the service, and returns `200 OK` with the JSON array.

### Design Decisions
- **In-memory seeding:** `GetNearbyRecyclingFacilitiesService` will declare a private `List<RecyclingFacility>` field and populate it with hard-coded sample data inside its parameterless constructor. This satisfies the rule that the service can be constructed directly in a unit test.
- **Lookup strategy:** For a given `zipCode`, the service will return a filtered subset of the seeded list whose entries are considered nearby (e.g., exact zip-code matches plus pre-defined neighbors). `DistanceInMiles` values are baked into the seeded data so the output remains deterministic.
- **DI wiring:** `Program.cs` registers `IGetNearbyRecyclingFacilitiesService` → `GetNearbyRecyclingFacilitiesService` (Singleton is acceptable because the state is immutable seeded data).
- **Synchronous contract:** The service method is declared without `async`/`Task` so the pre-written tests can call it directly.

---

## File List

| Relative Path | Purpose |
|---------------|---------|
| `Implementation.csproj` | .NET 8 Web SDK project file with assembly/name/root namespace set to `Implementation`. |
| `Program.cs` | Application bootstrap; wires routing, controllers, and the service interface implementation. |
| `Models/RecyclingFacility.cs` | Data model class in namespace `Implementation.Models`. |
| `Services/IGetNearbyRecyclingFacilitiesService.cs` | Service interface in namespace `Implementation.Services`. |
| `Services/GetNearbyRecyclingFacilitiesService.cs` | Service implementation in namespace `Implementation.Services`; parameterless constructor seeds in-memory facilities. |
| `Controllers/RecyclingFacilitiesController.cs` | ASP.NET Core controller exposing `GET /api/recycling-facilities`. |

---

## Naming Contract Compliance

| Rule Requirement | How It Is Satisfied |
|------------------|---------------------|
| **Project / Assembly / Root Namespace** | The project file is named `Implementation.csproj`. It contains `<AssemblyName>Implementation</AssemblyName>` and `<RootNamespace>Implementation</RootNamespace>`. |
| **Data Model** | `Implementation.Models.RecyclingFacility` is defined with the exact properties and types from the user story: `int FacilityId`, `string Name`, `string Address`, `string City`, `string State`, `string ZipCode`, `double DistanceInMiles`, `string PhoneNumber`. |
| **Service Interface** | `Implementation.Services.IGetNearbyRecyclingFacilitiesService` declares a single synchronous method: `IEnumerable<RecyclingFacility> GetNearbyRecyclingFacilities(string zipCode);`. |
| **Service Implementation** | `Implementation.Services.GetNearbyRecyclingFacilitiesService` implements the interface above. It provides a parameterless constructor that initializes an in-memory list of `RecyclingFacility` objects, requiring no external services. |
| **Controller / Route** | A controller (name unconstrained) maps `GET /api/recycling-facilities`, accepts a `zipCode` query string parameter, invokes the service method, and returns the `IEnumerable<RecyclingFacility>` as an HTTP 200 JSON array. |