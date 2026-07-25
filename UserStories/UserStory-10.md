# User Story 10

## Description
As a website user, I want to see updated financial assistance data daily.

---

# API Interface

## HTTP Method
GET

## Endpoint
`/api/financial-assistance-data`

## Method Name
GetFinancialAssistanceData

## Parameters

| Name | Data Type | Parameter Type | Required | Description |
|------|-----------|----------------|----------|-------------|
| agency | string | Query String | No | Filter results by awarding agency |
| fiscalYear | int | Query String | No | Filter results by fiscal year |
| page | int | Query String | No | Page number for paginated results |
| pageSize | int | Query String | No | Number of records to return per page |

## Response

**HTTP 200 OK**

```json
{
  "lastUpdatedDate": "2026-07-24T06:00:00Z",
  "records": [
    {
      "awardId": "ASST-2026-000345",
      "recipientName": "Springfield Community Services",
      "agency": "Department of Health and Human Services",
      "awardAmount": 125000.00,
      "awardDate": "2026-07-20T00:00:00Z"
    }
  ]
}
```

---

# Data Model

## FinancialAssistanceDataResponse

| Property | Type | Nullable | Description |
|----------|------|----------|--------------|
| LastUpdatedDate | datetime | Non-nullable | Date and time the financial assistance data was last refreshed |
| Records | list\<FinancialAssistanceRecord\> | Non-nullable | List of financial assistance award records |

## FinancialAssistanceRecord

| Property | Type | Nullable | Description |
|----------|------|----------|--------------|
| AwardId | string | Non-nullable | Unique identifier of the financial assistance award |
| RecipientName | string | Non-nullable | Name of the award recipient |
| Agency | string | Non-nullable | Awarding agency name |
| AwardAmount | decimal | Non-nullable | Dollar amount of the award |
| AwardDate | datetime | Non-nullable | Date the award was issued |
