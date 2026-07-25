# User Story 08

## Description
As a FABS user, I want to download the uploaded FABS file, so that I can get the uploaded file.

---

# API Interface

## HTTP Method
GET

## Endpoint
`/api/fabs/submissions/{submissionId}/file`

## Method Name
DownloadFabsFile

## Parameters

| Name | Data Type | Parameter Type | Required | Description |
|------|-----------|----------------|----------|-------------|
| submissionId | int | Route | Yes | Unique identifier of the FABS submission whose uploaded file should be downloaded |

## Response

**HTTP 200 OK**

```json
{
  "submissionId": 501,
  "fileName": "fabs_submission_501.csv",
  "fileSizeInBytes": 204800,
  "contentType": "text/csv",
  "downloadUrl": "https://files.example.gov/fabs/501/fabs_submission_501.csv"
}
```

---

# Data Model

## FabsFileDownload

| Property | Type | Nullable | Description |
|----------|------|----------|--------------|
| SubmissionId | int | Non-nullable | Unique identifier of the FABS submission |
| FileName | string | Non-nullable | Name of the originally uploaded file |
| FileSizeInBytes | long | Non-nullable | Size of the uploaded file in bytes |
| ContentType | string | Non-nullable | MIME type of the uploaded file |
| DownloadUrl | string | Non-nullable | URL from which the uploaded file can be downloaded |
