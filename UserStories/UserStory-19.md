# User Story 09

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

```json
{}
```

---

# Data Model

## CourseDeletionResult

| Property | Type | Nullable | Description |
|----------|------|----------|--------------|
| CourseId | int | Non-nullable | Unique identifier of the deleted course or event |
| Status | string | Non-nullable | Result status of the deletion (Deleted, NotFound, Forbidden) |
| DeletedAt | datetime | Nullable | Timestamp when the deletion completed; null if deletion failed |
