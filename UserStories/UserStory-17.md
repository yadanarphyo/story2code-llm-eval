# User Story 17

## Description
As a trainer, I want to create a new course or event, so that site visitors can see it.

---

# API Interface

## HTTP Method
POST

## Endpoint
`/api/courses`

## Method Name
CreateCourse

## Parameters

| Name | Data Type | Parameter Type | Required | Description |
|------|-----------|----------------|----------|-------------|
| title | string | Body | Yes | Title of the course or event |
| description | string | Body | No | Description of the course or event |
| startDate | datetime | Body | Yes | Date and time the course or event begins |
| endDate | datetime | Body | No | Date and time the course or event ends |
| location | string | Body | No | Physical or virtual location of the course or event |
| capacity | int | Body | No | Maximum number of attendees |

## Response

**HTTP 201 Created**

```json
{
  "courseId": 501,
  "title": "Introduction to Data Packaging",
  "status": "Published",
  "startDate": "2026-09-10T09:00:00Z",
  "createdAt": "2026-07-25T12:00:00Z"
}
```

---

# Data Model

## Course

| Property | Type | Nullable | Description |
|----------|------|----------|--------------|
| CourseId | int | Non-nullable | Unique identifier of the course or event |
| Title | string | Non-nullable | Title of the course or event |
| Description | string | Nullable | Description of the course or event |
| StartDate | datetime | Non-nullable | Date and time the course or event begins |
| EndDate | datetime | Nullable | Date and time the course or event ends |
| Location | string | Nullable | Physical or virtual location of the course or event |
| Capacity | int | Nullable | Maximum number of attendees; null if unlimited |
| Status | string | Non-nullable | Publication status of the course or event (Draft, Published, Cancelled) |
| CreatedAt | datetime | Non-nullable | Timestamp when the course or event was created |
