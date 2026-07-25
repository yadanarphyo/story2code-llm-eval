# User Story 19

## Description
As a trainer, I want to delete one of my courses or events, so that it's no longer listed if I cancel for some reason.

---

# API Interface

## HTTP Method
DELETE

## Endpoint
`/api/courses/{courseId}`

## Method Name
DeleteCourse

## Parameters

| Name | Data Type | Parameter Type | Required | Description |
|------|-----------|----------------|----------|-------------|
| courseId | int | Path | Yes | Unique identifier of the course or event to delete |

## Response

**HTTP 204 No Content**
