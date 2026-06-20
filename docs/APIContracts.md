# API Contracts

Pure request/response contract reference for every endpoint. For prose explanations, error codes context, and business rules, see [api/README.md](../api/README.md).

All responses are wrapped in the standard envelope:

```typescript
interface ApiResponse<T> {
  success: boolean;
  statusCode: number;
  message: string;
  data: T;
  errors: string[] | null;
  timestamp: string;
}
```

---

## Auth

### `POST /api/auth/register`
**Auth:** None
**Request:**
```typescript
{
  name: string;      // required, max 50 chars
  email: string;      // required, valid email format
  password: string;   // required
}
```
**Response `201`:** `ApiResponse<UserDTO>`
```typescript
{
  id: number;
  name: string;
  email: string;
  role: string;       // always "Customer" on registration
}
```
**Errors:** `400` — registration data missing or invalid

---

### `POST /api/auth/login`
**Auth:** None
**Request:**
```typescript
{
  email: string;      // required, valid email format
  password: string;   // required
}
```
**Response `200`:** `ApiResponse<LoginResponseDTO>`
```typescript
{
  token: string;
  userDTO: {
    id: number;
    name: string;
    email: string;
    role: string;
  };
}
```
**Errors:** `401` — invalid email or password

---

## User

### `GET /api/user/profile`
**Auth:** Any authenticated user
**Request:** None
**Response `200`:** `ApiResponse<UserDTO>`
```typescript
{
  id: number;
  name: string;
  email: string;
  role: string;
}
```
**Errors:** `401` — invalid/missing token

---

### `PATCH /api/user/profile`
**Auth:** Any authenticated user
**Request:**
```typescript
{
  name: string;      // required, max 50 chars
}
```
**Response `200`:** `ApiResponse<UserDTO>` (same shape as above)
**Errors:** `401` — invalid/missing token · `404` — user not found

---

## Book

### `GET /api/book`
**Auth:** Any authenticated user
**Request:** None
**Response `200`:** `ApiResponse<BookDTO[]>`
```typescript
{
  bookId: number;
  title: string;
  author: string;
  genre: string | null;
  description: string | null;
  averageRating: number | null;   // computed dynamically, null if unrated
}[]
```

---

### `GET /api/book/{id}`
**Auth:** Any authenticated user
**Path param:** `id: number`
**Request:** None
**Response `200`:** `ApiResponse<BookDTO>` (single object, shape as above)
**Errors:** `400` — id ≤ 0 · `404` — book not found

---

### `GET /api/book/search`
**Auth:** Any authenticated user
**Query params (all optional):**
```typescript
{
  query?: string;     // matches title, partial, case-insensitive
  author?: string;     // matches author, partial, case-insensitive
  genre?: string;      // matches genre, partial, case-insensitive
}
```
**Response `200`:** `ApiResponse<BookDTO[]>` (same shape as `GET /api/book`)

---

### `POST /api/book`
**Auth:**  Admin only
**Request:**
```typescript
{
  title: string;       // required
  author: string;       // required
  genre?: string;
  description?: string;
}
```
**Response `201`:** `ApiResponse<BookDTO>`
**Errors:** `400` — data missing · `409`/`400` — duplicate title (thrown as exception, caught by global middleware)

---

### `PUT /api/book/{id}`
**Auth:**  Admin only
**Path param:** `id: number`
**Request:**
```typescript
{
  id: number;          // required, must equal route {id}
  title: string;        // required
  author: string;        // required
  genre: string;
  description: string;
}
```
**Response `200`:** `ApiResponse<BookDTO>`
**Errors:** `400` — body missing or `id` mismatch with route · `404` — book not found

---

### `DELETE /api/book/{id}`
**Auth:**  Admin only
**Path param:** `id: number`
**Request:** None
**Response `200`:** `ApiResponse<object>` — no content body
**Errors:** `404` — book not found

---

## User Book Activity

> All endpoints below require role: **Customer**

### `POST /api/user/read`
**Request:**
```typescript
{
  bookId: number;       // required
}
```
**Response `200`:** `ApiResponse<object>`
**Errors:** `401` — invalid/missing token

---

### `POST /api/user/rate`
**Request:**
```typescript
{
  bookId: number;       // required
  rating: number;        // required, range 1–5
}
```
**Response `200`:** `ApiResponse<object>`
**Errors:** `401` — invalid/missing token

---

### `GET /api/user/history`
**Request:** None
**Response `200`:** `ApiResponse<UserHistoryDTO[]>`
```typescript
{
  bookId: number;
  title: string;
  author: string;
  genre: string | null;
  rating: number | null;     // null if user hasn't rated this book
  isRead: boolean;
}[]
```
**Errors:** `401` — invalid/missing token

---

## Recommendation

> Requires role: **Customer**

### `POST /api/recommendation`
**Request (all fields optional):**
```typescript
{
  prompt?: string;
  genre?: string;
  minimumRating?: number;
}
```
**Response `200`:** `ApiResponse<RecommendationResponseDTO>`
```typescript
{
  internalRecommendations: {
    bookId: number;
    title: string;
    author: string;
    genre: string | null;
    description: string | null;
    averageRating: number | null;
  }[];
  externalRecommendations: {
    title: string;
    author: string | null;
    genre: string | null;
    isInternal: boolean;        // always false in this array
    searchUrl: string;            // generated Google search link
  }[];
}
```
**Errors:** `500` — Groq API failure propagates as an unhandled exception (see [Assumptions & Limitations](./assumptions.md))

---

## Status Code Summary

| Code | Meaning | Used for |
|---|---|---|
| `200` | OK | Successful GET/POST/PUT/PATCH/DELETE |
| `201` | Created | Successful registration, book creation |
| `400` | Bad Request | Missing/invalid request data, ID mismatch |
| `401` | Unauthorized | Invalid credentials, missing/invalid/expired token |
| `404` | Not Found | Resource (book/user) doesn't exist |
| `500` | Internal Server Error | Unhandled exceptions (caught by global middleware) |