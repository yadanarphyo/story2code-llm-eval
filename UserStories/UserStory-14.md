# User Story 14

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
