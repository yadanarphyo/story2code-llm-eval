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

---

# Data Model

## Dataset

Candidate dataset the delete operates over (the method itself returns no body).

| Property | Type | Nullable | Description |
|----------|------|----------|--------------|
| DatasetId | int | Non-nullable | Unique identifier of the dataset |
| Name | string | Non-nullable | Name of the dataset |
| Description | string | Nullable | Description of the dataset |
| Publisher | string | Nullable | Name of the organization or user who published the dataset |
| Status | string | Non-nullable | Publication status of the dataset (Published, Draft, Archived) |
| CreatedAt | datetime | Non-nullable | Timestamp when the dataset was published |
