# User Story 01

## Description
As a user, I want to be able to enter my zip code and get a list of nearby recycling facilities, so that I can determine which ones I should consider.

---

# API Interface

## HTTP Method
GET

## Endpoint
`/api/recycling-facilities`

## Method Name
GetNearbyRecyclingFacilities

## Parameters

| Name | Data Type | Parameter Type | Required | Description |
|------|-----------|----------------|----------|-------------|
| zipCode | string | Query String | Yes | The user's postal/zip code used to search for nearby recycling facilities |

## Response

**HTTP 200 OK**

```json
[
  {
    "facilityId": 1,
    "name": "Green Recycling Center",
    "address": "123 Main Street",
    "city": "Springfield",
    "state": "IL",
    "zipCode": "62701",
    "distanceInMiles": 2.8,
    "phoneNumber": "(555) 123-4567"
  }
]
```

---

# Data Model

## RecyclingFacility

| Property | Type | Description |
|----------|------|--------------|
| FacilityId | int | Unique identifier of the recycling facility |
| Name | string | Facility name |
| Address | string | Street address |
| City | string | City |
| State | string | State or region |
| ZipCode | string | Postal/Zip code |
| DistanceInMiles | double | Distance from the searched zip code |
| PhoneNumber | string | Contact phone number |
