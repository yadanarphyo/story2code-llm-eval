# User Story 08

## Description
As a trainer, I want to update one of my existing courses or events, so that it reflects accurate information.

---

# API Interface

## HTTP Method
PUT

## Endpoint
`/api/courses/{courseId}`

## Method Name
UpdateCourse

## Parameters

| Name | Data Type | Parameter Type | Required | Description |
|------|-----------|----------------|----------|-------------|
| courseId | int | Path | Yes | Unique identifier of the course or event to update |
| title | string | Body | No | Updated title of the course or event |
| description | string | Body | No | Updated description of the course or event |
| startDate | datetime | Body | No | Updated date and time the course or event begins |
| endDate | datetime | Body | No | Updated date and time the course or event ends |
| location | string | Body | No | Updated physical or virtual location |
| capacity | int | Body | No | Updated maximum number of attendees |

## Response

**HTTP 200 OK**

```json
{
  "courseId": 501,
  "title": "Introduction to Data Packaging (Updated)",
  "status": "Published",
  "updatedAt": "2026-07-25T13:15:00Z"
}
```

---

# Data Model

## CourseUpdateResult

| Property | Type | Nullable | Description |
|----------|------|----------|--------------|
| CourseId | int | Non-nullable | Unique identifier of the course or event |
| Title | string | Non-nullable | Current title of the course or event |
| Status | string | Non-nullable | Publication status of the course or event (Draft, Published, Cancelled) |
| UpdatedAt | datetime | Non-nullable | Timestamp of the most recent update |
