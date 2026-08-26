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

---

# Data Model

## Course

Candidate dataset the delete operates over (the method itself returns no body). Same shape as
the `Course` model in US-17 (CreateCourse), since it's the same entity.

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
