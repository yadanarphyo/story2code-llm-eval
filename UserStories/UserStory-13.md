# User Story 03

## Description
As a Data Publishing User, I want to be able to edit a dataset I have published, So that I can correct or enhance existing data.

---

# API Interface

## HTTP Method
PUT

## Endpoint
`/api/datasets/{datasetId}`

## Method Name
UpdateDataset

## Parameters

| Name | Data Type | Parameter Type | Required | Description |
|------|-----------|----------------|----------|-------------|
| datasetId | int | Path | Yes | Unique identifier of the dataset to edit |
| name | string | Body | No | Updated name of the dataset |
| description | string | Body | No | Updated description of the dataset |
| metadata | object | Body | No | Updated metadata key/value pairs for the dataset |
| rows | array | Body | No | Updated row-level data to replace or merge into the dataset |

## Response

**HTTP 200 OK**

```json
{
  "datasetId": 101,
  "name": "Municipal Budget 2026 (Revised)",
  "description": "Corrected figures for Q2 spending",
  "status": "Published",
  "updatedAt": "2026-07-25T11:30:00Z"
}
```

---

# Data Model

## DatasetUpdateResult

| Property | Type | Nullable | Description |
|----------|------|----------|--------------|
| DatasetId | int | Non-nullable | Unique identifier of the dataset |
| Name | string | Non-nullable | Current name of the dataset |
| Description | string | Nullable | Current description of the dataset |
| Status | string | Non-nullable | Publication status of the dataset (Published, Draft, Archived) |
| UpdatedAt | datetime | Non-nullable | Timestamp of the most recent update |
