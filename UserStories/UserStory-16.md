# User Story 16

## Description
As a Data Consuming User, I want to be able to search any dataset published and publicly accessible by their title and metadata, So that I can find the datasets I'm interested in.

---

# API Interface

## HTTP Method
GET

## Endpoint
`/api/datasets/search`

## Method Name
SearchPublicDatasets

## Parameters

| Name | Data Type | Parameter Type | Required | Description |
|------|-----------|----------------|----------|-------------|
| query | string | Query String | Yes | Free-text search term matched against dataset title and metadata |
| page | int | Query String | No | Page number of results to return; defaults to 1 |
| pageSize | int | Query String | No | Number of results per page; defaults to 20 |

## Response

**HTTP 200 OK**

```json
{
  "totalResults": 2,
  "page": 1,
  "pageSize": 20,
  "results": [
    {
      "datasetId": 101,
      "title": "Municipal Budget 2026",
      "publisher": "City of Springfield",
      "tags": ["budget", "municipal", "2026"],
      "publishedAt": "2026-06-01T09:00:00Z"
    }
  ]
}
```

---

# Data Model

## DatasetSearchResult

| Property | Type | Nullable | Description |
|----------|------|----------|--------------|
| DatasetId | int | Non-nullable | Unique identifier of the matching dataset |
| Title | string | Non-nullable | Title of the dataset |
| Publisher | string | Nullable | Name of the organization or user who published the dataset |
| Tags | array of string | Nullable | Metadata tags associated with the dataset |
| PublishedAt | datetime | Non-nullable | Timestamp when the dataset was published |

## DatasetSearchResponse

| Property | Type | Nullable | Description |
|----------|------|----------|--------------|
| TotalResults | int | Non-nullable | Total number of datasets matching the search query |
| Page | int | Non-nullable | Current page number of results |
| PageSize | int | Non-nullable | Number of results returned per page |
| Results | array of DatasetSearchResult | Non-nullable | Collection of matching datasets for the current page |
