# User Story 01

## Description
As a Data Publishing User, I want to be able to import data in JSON, So that I do not have to convert data formats in order to use the data packager.

---

# API Interface

## HTTP Method
POST

## Endpoint
`/api/datasets/import/json`

## Method Name
ImportDatasetFromJson

## Parameters

| Name | Data Type | Parameter Type | Required | Description |
|------|-----------|----------------|----------|-------------|
| file | file (application/json) | Form Data | Yes | The JSON file containing the dataset to import |
| datasetName | string | Form Data | No | Optional name to assign to the imported dataset |

## Response

**HTTP 201 Created**

```json
{
  "datasetId": 101,
  "name": "Municipal Budget 2026",
  "sourceFormat": "JSON",
  "status": "Processing",
  "rowCount": 4520,
  "createdAt": "2026-07-25T10:00:00Z"
}
```

---

# Data Model

## ImportedDataset

| Property | Type | Nullable | Description |
|----------|------|----------|--------------|
| DatasetId | int | Non-nullable | Unique identifier of the imported dataset |
| Name | string | Non-nullable | Name of the dataset |
| SourceFormat | string | Non-nullable | Original format of the imported data (e.g., JSON) |
| Status | string | Non-nullable | Processing status of the import (Processing, Completed, Failed) |
| RowCount | int | Nullable | Number of rows detected in the imported file; null until processing completes |
| CreatedAt | datetime | Non-nullable | Timestamp when the import was initiated |
