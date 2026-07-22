# User Story 02

## Description
As a user, I want to be able to create an account, so that I can create my own profile.

---

# API Interface

## HTTP Method
POST

## Endpoint
`/api/accounts`

## Method Name
CreateAccount

## Parameters

| Name | Data Type | Parameter Type | Required | Description |
|------|-----------|----------------|----------|-------------|
| email | string | Body | Yes | The email address used to register the account |
| password | string | Body | Yes | The password for the account |
| confirmPassword | string | Body | Yes | Confirmation of the password, must match `password` |
| displayName | string | Body | No | The name to display on the user's profile |

## Response

**HTTP 201 Created**

```json
{
  "accountId": 101,
  "email": "user@example.com",
  "displayName": "New User",
  "createdAt": "2026-07-21T09:00:00Z"
}
```

---

# Data Model

## Account

| Property | Type | Nullable | Description |
|----------|------|----------|--------------|
| AccountId | int | Non-nullable | Unique identifier of the account |
| Email | string | Non-nullable | Registered email address |
| PasswordHash | string | Non-nullable | Hashed password for authentication |
| DisplayName | string | Nullable | Name shown on the user's profile |
| CreatedAt | datetime | Non-nullable | Date and time the account was created |
