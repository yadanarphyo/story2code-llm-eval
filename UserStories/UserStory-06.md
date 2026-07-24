# User Story 06

## Description
As a user, I want to upload my week's schedule, so that I can get recommendations for recycling centers that best fit my availability.

---

# API Interface

## HTTP Method
POST

## Endpoint
`/api/users/{userId}/schedule`

## Method Name
UploadUserSchedule

## Parameters

| Name | Data Type | Parameter Type | Required | Description |
|------|-----------|----------------|----------|-------------|
| userId | int | Route | Yes | Unique identifier of the user |
| availability | array | Body | Yes | List of available time slots for the week |

## Response

**HTTP 201 Created**

```json
{
  "scheduleId": 7,
  "userId": 45,
  "availability": [
    { "dayOfWeek": "Monday", "startTime": "09:00", "endTime": "12:00" },
    { "dayOfWeek": "Wednesday", "startTime": "14:00", "endTime": "18:00" }
  ]
}
```

---

# Data Model

## UserSchedule

| Property | Type | Nullable | Description |
|----------|------|----------|--------------|
| ScheduleId | int | Non-nullable | Unique identifier of the uploaded schedule |
| UserId | int | Non-nullable | Identifier of the user the schedule belongs to |
| Availability | list\<TimeSlot\> | Non-nullable | List of available time slots for the week |

## TimeSlot

| Property | Type | Nullable | Description |
|----------|------|----------|--------------|
| DayOfWeek | string | Non-nullable | Day of the week for the time slot |
| StartTime | time | Non-nullable | Start time of availability |
| EndTime | time | Non-nullable | End time of availability |
