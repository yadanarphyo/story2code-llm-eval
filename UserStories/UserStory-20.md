# User Story 10

## Description
As a trainer, I want to copy one of my courses or events, so that I can create a new one.

---

# API Interface

## HTTP Method
POST

## Endpoint
`/api/courses/{courseId}/copy`

## Method Name
CopyCourse

## Parameters

| Name | Data Type | Parameter Type | Required | Description |
|------|-----------|----------------|----------|-------------|
| courseId | int | Path | Yes | Unique identifier of the course or event to copy |
| title | string | Body | No | Title to assign to the new copy; defaults to "Copy of {original title}" if omitted |
| startDate | datetime | Body | No | Start date and time for the new copy |

## Response

**HTTP 201 Created**

```json
{
  "courseId": 502,
  "copiedFromCourseId": 501,
  "title": "Copy of Introduction to Data Packaging",
  "status": "Draft",
  "createdAt": "2026-07-25T14:00:00Z"
}
```

---

# Data Model

## CourseCopyResult

| Property | Type | Nullable | Description |
|----------|------|----------|--------------|
| CourseId | int | Non-nullable | Unique identifier of the newly created course or event copy |
| CopiedFromCourseId | int | Non-nullable | Unique identifier of the original course or event that was copied |
| Title | string | Non-nullable | Title of the new copy |
| Status | string | Non-nullable | Publication status of the new copy (Draft, Published, Cancelled) |
| CreatedAt | datetime | Non-nullable | Timestamp when the copy was created |
