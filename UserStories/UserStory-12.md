# User Story 02

## Description
As a Data Publishing User, I want to be able to import data in Excel, So that I do not have to convert data formats in order to use the data packager.

---

# API Interface

## HTTP Method
POST

## Endpoint
`/api/datasets/import/excel`

## Method Name
ImportDatasetFromExcel

## Parameters

| Name | Data Type | Parameter Type | Required | Description |
|------|-----------|----------------|----------|-------------|
| file | file (.xlsx/.xls) | Form Data | Yes | The Excel workbook containing the dataset to import |
| sheetName | string | Form Data | No | Name of the worksheet to import; defaults to the first sheet if omitted |
| datasetName | string | Form Data | No | Optional name to assign to the imported dataset |

## Response

**HTTP 201 Created**

```json
{
  "datasetId": 102,
  "name": "Regional Sales Q2",
  "sourceFormat": "Excel",
  "sheetName": "Sheet1",
  "status": "Processing",
  "rowCount": 1875,
  "createdAt": "2026-07-25T10:05:00Z"
}
```

---

# Data Model

## ImportedDataset

| Property | Type | Nullable | Description |
|----------|------|----------|--------------|
| DatasetId | int | Non-nullable | Unique identifier of the imported dataset |
| Name | string | Non-nullable | Name of the dataset |
| SourceFormat | string | Non-nullable | Original format of the imported data (e.g., Excel) |
| SheetName | string | Nullable | Name of the worksheet that was imported |
| Status | string | Non-nullable | Processing status of the import (Processing, Completed, Failed) |
| RowCount | int | Nullable | Number of rows detected in the imported sheet; null until processing completes |
| CreatedAt | datetime | Non-nullable | Timestamp when the import was initiated |
