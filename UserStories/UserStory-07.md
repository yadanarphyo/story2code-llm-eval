# User Story 07

## Description
As a recycling facility representative, I want to have access to user stats and schedules, so that I can adjust my hours and/or upgrade equipment and capacity in order to be able to accommodate larger amounts of recyclable materials.

---

# API Interface

## HTTP Method
GET

## Endpoint
`/api/recycling-facilities/{facilityId}/user-stats`

## Method Name
GetFacilityUserStats

## Parameters

| Name | Data Type | Parameter Type | Required | Description |
|------|-----------|----------------|----------|-------------|
| facilityId | int | Route | Yes | Unique identifier of the recycling facility |
| startDate | date | Query String | No | Only include data on or after this date |
| endDate | date | Query String | No | Only include data on or before this date |

## Response

**HTTP 200 OK**

```json
{
  "facilityId": 1,
  "totalVisits": 342,
  "peakDayOfWeek": "Saturday",
  "peakHourRange": "10:00-12:00",
  "aggregatedUserAvailability": [
    { "dayOfWeek": "Saturday", "startTime": "09:00", "endTime": "13:00", "userCount": 58 }
  ]
}
```

---

# Data Model

## FacilityUserStats

| Property | Type | Nullable | Description |
|----------|------|----------|--------------|
| FacilityId | int | Non-nullable | Unique identifier of the recycling facility |
| TotalVisits | int | Non-nullable | Total number of user visits within the requested date range |
| PeakDayOfWeek | string | Nullable | Day of the week with the highest user demand |
| PeakHourRange | string | Nullable | Time range with the highest user demand |
| AggregatedUserAvailability | list\<AvailabilityStat\> | Non-nullable | Aggregated count of users available during each time slot |

## AvailabilityStat

| Property | Type | Nullable | Description |
|----------|------|----------|--------------|
| DayOfWeek | string | Non-nullable | Day of the week for the aggregated slot |
| StartTime | time | Non-nullable | Start time of the aggregated slot |
| EndTime | time | Non-nullable | End time of the aggregated slot |
| UserCount | int | Non-nullable | Number of users available during this slot |
