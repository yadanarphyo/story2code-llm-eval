# User Story 03

## Description
As a user, I want to choose a flexible pick up time, so that I can more conveniently use the website.

---

# API Interface

## HTTP Method
POST

## Endpoint
`/api/pickup-time-preferences`

## Method Name
SetFlexiblePickupTime

## Parameters

| Name | Data Type | Parameter Type | Nullable | Required | Description |
|------|-----------|----------------|----------|----------|-------------|
| orderId | int | Body | No | Yes | Identifier of the order or booking the pickup time applies to |
| preferredDate | date | Body | No | Yes | The date the user wants to pick up |
| timeWindowStart | time | Body | No | Yes | Earliest time in the user's preferred pickup window |
| timeWindowEnd | time | Body | No | Yes | Latest time in the user's preferred pickup window |
| isFlexible | bool | Body | No | Yes | Indicates whether the user is open to alternate time slots near the preferred window |
| notes | string | Body | Yes | No | Optional notes from the user about pickup constraints |

## Response

**HTTP 201 Created**

```json
{
  "preferenceId": 501,
  "orderId": 1024,
  "preferredDate": "2026-07-25",
  "timeWindowStart": "09:00",
  "timeWindowEnd": "12:00",
  "isFlexible": true,
  "notes": null,
  "confirmedPickupTime": null,
  "status": "Pending",
  "createdAt": "2026-07-22T10:15:00Z"
}
```

---

# Data Model

## PickupTimePreference

| Property | Type | Nullable | Description |
|----------|------|----------|--------------|
| PreferenceId | int | No | Unique identifier of the pickup time preference |
| OrderId | int | No | Identifier of the associated order or booking |
| PreferredDate | date | No | Date the user wants to pick up |
| TimeWindowStart | time | No | Earliest time in the user's preferred pickup window |
| TimeWindowEnd | time | No | Latest time in the user's preferred pickup window |
| IsFlexible | bool | No | Whether the user accepts alternate slots close to the preferred window |
| Notes | string | Yes | Optional notes provided by the user |
| ConfirmedPickupTime | datetime | Yes | Final pickup time confirmed by the system; null until assigned |
| Status | string | No | Current status of the preference (e.g., Pending, Confirmed, Cancelled) |
| CreatedAt | datetime | No | Timestamp when the preference was created |
