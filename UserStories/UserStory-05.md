# User Story 05

## Description
As an admin, I want to be able to read users' feedback and complaints, so that we can add more features and keep improving the service we provide to them.

---

# API Interface

## HTTP Method
GET

## Endpoint
`/api/feedback`

## Method Name
GetUserFeedback

## Parameters

| Name | Data Type | Parameter Type | Required | Description |
|------|-----------|----------------|----------|-------------|
| status | string | Query String | No | Filter feedback by status (e.g. "New", "Reviewed", "Resolved") |
| type | string | Query String | No | Filter by type (e.g. "Feedback", "Complaint") |
| startDate | date | Query String | No | Only return entries submitted on or after this date |
| endDate | date | Query String | No | Only return entries submitted on or before this date |

## Response

**HTTP 200 OK**

```json
[
  {
    "feedbackId": 1,
    "userId": 45,
    "type": "Complaint",
    "subject": "Incorrect facility hours",
    "message": "The listed hours for Green Recycling Center were wrong.",
    "status": "New",
    "submittedDate": "2026-07-01T09:15:00Z"
  }
]
```

---

# Data Model

## UserFeedback

| Property | Type | Nullable | Description |
|----------|------|----------|--------------|
| FeedbackId | int | Non-nullable | Unique identifier of the feedback or complaint entry |
| UserId | int | Non-nullable | Identifier of the user who submitted the entry |
| Type | string | Non-nullable | Category of the entry ("Feedback" or "Complaint") |
| Subject | string | Non-nullable | Short summary of the entry |
| Message | string | Non-nullable | Full text of the feedback or complaint |
| Status | string | Non-nullable | Current review status of the entry |
| SubmittedDate | datetime | Non-nullable | Date and time the entry was submitted |
