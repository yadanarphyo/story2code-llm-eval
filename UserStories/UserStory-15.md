# User Story 15

## Description
As a Data Consuming User, I want to be able to download an image of a particular view state, So that I can use it offline.

---

# API Interface

## HTTP Method
GET

## Endpoint
`/api/views/{viewId}/image`

## Method Name
DownloadViewImage

## Parameters

| Name | Data Type | Parameter Type | Required | Description |
|------|-----------|----------------|----------|-------------|
| viewId | int | Path | Yes | Unique identifier of the view whose state should be rendered |
| format | string | Query String | No | Desired image format (png, jpeg, svg); defaults to png |
| width | int | Query String | No | Desired image width in pixels |
| height | int | Query String | No | Desired image height in pixels |

## Response

**HTTP 200 OK**

```
Content-Type: image/png

<binary image data>
```

---

# Data Model

## ViewImageExport

| Property | Type | Nullable | Description |
|----------|------|----------|--------------|
| ViewId | int | Non-nullable | Unique identifier of the view that was rendered |
| Format | string | Non-nullable | Image format of the exported file (png, jpeg, svg) |
| Width | int | Nullable | Width of the exported image in pixels; null if not specified |
| Height | int | Nullable | Height of the exported image in pixels; null if not specified |
| FileSizeBytes | long | Non-nullable | Size of the generated image file in bytes |
| GeneratedAt | datetime | Non-nullable | Timestamp when the image was generated |
