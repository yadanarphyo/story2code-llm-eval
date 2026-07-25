# User Story 04

## Description
As a Data Publishing User, I want to be able to delete a dataset I have published, So that I can remove unwanted data from OpenSpending.

---

# API Interface

## HTTP Method
DELETE

## Endpoint
`/api/datasets/{datasetId}`

## Method Name
DeleteDataset

## Parameters

| Name | Data Type | Parameter Type | Required | Description |
|------|-----------|----------------|----------|-------------|
| datasetId | int | Path | Yes | Unique identifier of the dataset to delete |

## Response

**HTTP 204 No Content**

```json
{}
```

---

# Data Model

## DatasetDeletionResult

| Property | Type | Nullable | Description |
|----------|------|----------|--------------|
| DatasetId | int | Non-nullable | Unique identifier of the deleted dataset |
| Status | string | Non-nullable | Result status of the deletion (Deleted, NotFound, Forbidden) |
| DeletedAt | datetime | Nullable | Timestamp when the deletion completed; null if deletion failed |
