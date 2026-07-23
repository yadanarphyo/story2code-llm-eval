# User Story 04

## Description
As an admin, I want to be able to add or remove recycling facilities' information, so that users get the most recent information.

---

# API Interface

## HTTP Method
PUT

## Endpoint
`/api/recycling-facilities/{facilityId}`

## Method Name
UpdateRecyclingFacility

## Parameters

| Name | Data Type | Parameter Type | Required | Description |
|------|-----------|----------------|----------|-------------|
| facilityId | int | Route | Yes | Unique identifier of the recycling facility to update |
| name | string | Body | Yes | Facility name |
| address | string | Body | Yes | Street address |
| city | string | Body | Yes | City |
| state | string | Body | Yes | State or region |
| zipCode | string | Body | Yes | Postal/Zip code |
| phoneNumber | string | Body | No | Contact phone number |

## Response

**HTTP 200 OK**

```json
{
  "facilityId": 1,
  "name": "Green Recycling Center",
  "address": "123 Main Street",
  "city": "Springfield",
  "state": "IL",
  "zipCode": "62701",
  "phoneNumber": "(555) 123-4567"
}
```

---

# Data Model

## RecyclingFacility

| Property | Type | Nullable | Description |
|----------|------|----------|--------------|
| FacilityId | int | Non-nullable | Unique identifier of the recycling facility |
| Name | string | Non-nullable | Facility name |
| Address | string | Non-nullable | Street address |
| City | string | Non-nullable | City |
| State | string | Non-nullable | State or region |
| ZipCode | string | Non-nullable | Postal/Zip code |
| PhoneNumber | string | Nullable | Contact phone number |
