# Project Context

Application:
Todo Management API

Architecture:
Clean Architecture

Database:
SQL Server

Authentication:
JWT (RSA256)

Coding Standards

- Use async/await
- Repository Pattern
- Unit Test Required

Current Feature

Implement User Authentication

Completed

- Login API
- Register API
- Refresh Token

Pending

- Forgot Password

---

# API Documentation

## Base URL

```
https://localhost:{port}
```

## Authentication

The API uses JWT Bearer authentication with RSA-SHA256 signing. Include the token in the `Authorization` header for protected endpoints:

```
Authorization: Bearer <token>
```

Tokens expire after 60 minutes (configurable via `Jwt:ExpirationMinutes`).

---

## Endpoints

### 1. POST /api/Auth/register

Register a new user account.

**Authentication:** None

**Request Body:**

| Field    | Type   | Required | Validation                  |
|----------|--------|----------|-----------------------------|
| username | string | Yes      | 3-50 characters             |
| email    | string | Yes      | Valid email format           |
| password | string | Yes      | 6-100 characters            |

**Example Request:**

```json
{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "SecurePass123"
}
```

**Success Response (201 Created):**

```json
{
  "token": "eyJhbGciOiJSUzI1NiIs...",
  "expiration": "2026-06-29T11:00:00Z",
  "refreshToken": "c3VwZXJzZWNyZXRyZWZyZXNo...",
  "refreshTokenExpiration": "2026-07-06T10:00:00Z",
  "username": "johndoe",
  "email": "john@example.com"
}
```

**Error Response (409 Conflict):**

```json
{
  "message": "Username already exists."
}
```

---

### 2. POST /api/Auth/login

Authenticate an existing user.

**Authentication:** None

**Request Body:**

| Field    | Type   | Required |
|----------|--------|----------|
| username | string | Yes      |
| password | string | Yes      |

**Example Request:**

```json
{
  "username": "johndoe",
  "password": "SecurePass123"
}
```

**Success Response (200 OK):**

```json
{
  "token": "eyJhbGciOiJSUzI1NiIs...",
  "expiration": "2026-06-29T11:00:00Z",
  "refreshToken": "c3VwZXJzZWNyZXRyZWZyZXNo...",
  "refreshTokenExpiration": "2026-07-06T10:00:00Z",
  "username": "johndoe",
  "email": "john@example.com"
}
```

**Error Response (401 Unauthorized):**

```json
{
  "message": "Invalid username or password."
}
```

---

### 3. POST /api/Auth/refresh

Exchange a valid refresh token for a new access token and refresh token (token rotation).

**Authentication:** None

**Request Body:**

| Field        | Type   | Required | Description                     |
|--------------|--------|----------|---------------------------------|
| refreshToken | string | Yes      | A valid, non-expired refresh token |

**Example Request:**

```json
{
  "refreshToken": "c3VwZXJzZWNyZXRyZWZyZXNo..."
}
```

**Success Response (200 OK):**

```json
{
  "token": "eyJhbGciOiJSUzI1NiIs...",
  "expiration": "2026-06-29T12:00:00Z",
  "refreshToken": "bmV3cmVmcmVzaHRva2VuLi4u...",
  "refreshTokenExpiration": "2026-07-06T11:00:00Z",
  "username": "johndoe",
  "email": "john@example.com"
}
```

**Error Response (401 Unauthorized):**

```json
{
  "message": "Invalid or expired refresh token."
}
```

**Notes:**
- The old refresh token is revoked upon use (rotation). Each refresh token can only be used once.
- A new refresh token is issued with each refresh, extending the session.

---

### 4. POST /api/Auth/revoke

Revoke a refresh token (logout).

**Authentication:** None

**Request Body:**

| Field        | Type   | Required | Description              |
|--------------|--------|----------|--------------------------|
| refreshToken | string | Yes      | The refresh token to revoke |

**Example Request:**

```json
{
  "refreshToken": "c3VwZXJzZWNyZXRyZWZyZXNo..."
}
```

**Success Response (200 OK):**

```json
{
  "message": "Token revoked."
}
```

---

### 5. GET /WeatherForecast

Retrieve a 5-day weather forecast (sample protected endpoint).

**Authentication:** Required (JWT Bearer)

**Request Body:** None

**Success Response (200 OK):**

```json
[
  {
    "date": "2026-06-30",
    "temperatureC": 25,
    "temperatureF": 76,
    "summary": "Warm"
  }
]
```

**Error Response (401 Unauthorized):** Returned when no valid token is provided.

---

## JWT Token Claims

| Claim | Description            |
|-------|------------------------|
| sub   | User ID (GUID)         |
| unique_name | Username         |
| email | User email address     |
| jti   | Unique token ID (GUID) |

## Configuration (appsettings.json)

| Key                            | Default          | Description                        |
|--------------------------------|------------------|------------------------------------|
| Jwt:Issuer                     | MemoryShareCheck | Token issuer identifier            |
| Jwt:Audience                   | MemoryShareCheck | Token audience identifier          |
| Jwt:ExpirationMinutes          | 60               | Access token lifetime in minutes   |
| Jwt:RefreshTokenExpirationDays | 7                | Refresh token lifetime in days     |
