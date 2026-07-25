# User Story 09

## Description
As an agency user, I want to know when the submission periods start and end, so that I know when the submission starts and ends.

---

# API Interface

## HTTP Method
GET

## Endpoint
`/api/submission-periods`

## Method Name
GetSubmissionPeriods

## Parameters

| Name | Data Type | Parameter Type | Required | Description |
|------|-----------|----------------|----------|-------------|
| fiscalYear | int | Query String | No | Filter submission periods by fiscal year |
| isCurrent | boolean | Query String | No | If true, only return the currently active submission period |

## Response

**HTTP 200 OK**

```json
[
  {
    "submissionPeriodId": 3,
    "fiscalYear": 2026,
    "period": "Q3",
    "startDate": "2026-07-01T00:00:00Z",
    "endDate": "2026-07-31T23:59:59Z",
    "isCurrent": true
  }
]
```

---

# Data Model

## SubmissionPeriod

| Property | Type | Nullable | Description |
|----------|------|----------|--------------|
| SubmissionPeriodId | int | Non-nullable | Unique identifier of the submission period |
| FiscalYear | int | Non-nullable | Fiscal year the submission period belongs to |
| Period | string | Non-nullable | Label identifying the period (e.g. "Q3") |
| StartDate | datetime | Non-nullable | Date and time the submission period opens |
| EndDate | datetime | Non-nullable | Date and time the submission period closes |
| IsCurrent | boolean | Non-nullable | Indicates whether this is the currently active submission period |
