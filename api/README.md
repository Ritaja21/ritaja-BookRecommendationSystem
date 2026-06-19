#  Backend — AI Book Recommendation System (api/)
 
ASP.NET Core Web API powering the Book Recommendation System. Built using a Repository → Service → Controller architecture, with JWT authentication, role-based authorization, and a hybrid AI recommendation engine via the Groq API.
 
---
## Tech Stack
 
| Component | Technology |
|---|---|
| Framework | ASP.NET Core Web API (.NET) |
| ORM | Entity Framework Core |
| Database | SQL Server |
| Auth | JWT Bearer tokens, BCrypt.Net for password hashing |
| Mapping | AutoMapper |
| AI Provider | Groq API (`llama-3.3-70b-versatile`) |
| Testing | xUnit, Moq, FluentAssertions |
 
---
## Project Structure
```
api/
├── Controllers/                    # API endpoints (Auth, Book, User, UserBook, Recommendation)
├── Services/                       # Business logic layer
├── Repositories/                   # Data access layer (EF Core queries)
├── Models/
│   ├── DTO/                         # Request/response data transfer objects
│   └── *.cs                        # Entity models (Book, User, UserBook)
├── Data/                            # AppDbContext, EF configuration
├── Mapping/                         # AutoMapper profiles
├── Middlewares/                     # Custom middleware (e.g. global exception handling)
├── Migrations/                      # EF Core migrations
├── Program.cs                       # App entry point & DI configuration
├── appsettings.json                 # containing the jwt and ai api keys
│
└── BookRecommendation.Tests/        # xUnit test project
├── Controllers/                  # Controller-level tests
├── Services/                     # Service-level tests (Auth, Book, UserBook)
└── GlobalUsings.cs
```

---

## Configuration Setup

`appsettings.json` **is committed** to this repository, but only with placeholder/empty values for sensitive fields — it exists so the configuration structure is visible to anyone cloning the repo.

`appsettings.Development.json` is **gitignored** and is where you should put your real secrets locally. ASP.NET Core automatically merges it over `appsettings.json` when running in the Development environment.

**Committed `api/appsettings.json` (placeholders):**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  "JwtSettings": {
    "Secret": ""
  },
  "GroqSettings": {
    "ApiKey": ""
  },
  "AllowedHosts": "*"
}
```

**Create your own `api/appsettings.Development.json` (gitignored, real values):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=BookRecommendationDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "Secret": "YOUR_JWT_SECRET_MIN_32_CHARACTERS"
  },
  "GroqSettings": {
    "ApiKey": "YOUR_GROQ_API_KEY"
  }
}
```

**Where to get each value:**

| Key | How to get it |
|---|---|
| `ConnectionStrings:DefaultConnection` | Your local or remote SQL Server instance connection string |
| `JwtSettings:Secret` | Any random string — **must be at least 32 characters** (HMAC-SHA256 requires a 256-bit key) |
| `GroqSettings:ApiKey` | Free API key from [console.groq.com/keys](https://console.groq.com/keys) |

> ⚠️ Never put real secrets in `appsettings.json` since it's committed. Always use `appsettings.Development.json` for real local values, and never remove it from `.gitignore`.

---
## Setup & Run
 
```bash
# from the api/ folder
dotnet restore
 
# apply EF Core migrations to create the database
dotnet ef database update
 
# run the API
dotnet run
```
 
The API will start on the port configured in `Properties/launchSettings.json` (typically `https://localhost:7xxx`). Swagger/Scalar UI is available at `/scalar` or `/swagger` depending on configuration, for interactive testing.
 
---
## Authentication & Roles
 
- Authentication uses **JWT Bearer tokens**. On successful login, the API returns a token containing the user's `Id`, `Name`, `Email`, and `Role` as claims.
- Two roles exist: **Customer** and **Admin**.
- **Customer accounts** are created through `/api/auth/register`. There is **no admin registration endpoint** — admin accounts must be created directly in the database for security.
- Protected endpoints expect the token in the `Authorization` header:
```
  Authorization: Bearer <token>
```
 
---
## API Response Format
 
Every endpoint returns a consistent envelope via `ApiResponse<TData>`:
 
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Description of what happened",
  "data": { },
  "errors": null,
  "timestamp": "2026-06-19T10:00:00.000Z"
}
```
 
On failure, `success` is `false`, `data` is typically `null`, and `errors` may contain additional detail.
 
---
## API Endpoints
 
### Auth — `/api/auth`
 
| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/api/auth/register` | None | Register a new customer account |
| POST | `/api/auth/login` | None | Log in and receive a JWT |
 
<details>
<summary><strong>POST /api/auth/register</strong></summary>

**Request body** (`RegisterRequestDTO`):
```json
{
  "name": "Jack Mathews",
  "email": "jack@example.com",
  "password": "Password123"
}
```
 
**Response** — `201 Created`:
```json
{
  "success": true,
  "statusCode": 201,
  "message": "User registered successfully",
  "data": {
    "id": 1,
    "name": "Jack Mathews",
    "email": "jack@example.com",
    "role": "Customer"
  },
  "errors": null,
  "timestamp": "2026-06-19T10:00:00.000Z"
}
```
</details>
<details>
<summary><strong>POST /api/auth/login</strong></summary>

**Request body** (`LoginRequestDTO`):
```json
{
  "email": "jack@example.com",
  "password": "Password123"
}
```
 
**Response** — `200 OK`:
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "userDTO": {
      "id": 1,
      "name": "Jack Mathews",
      "email": "jack@example.com",
      "role": "Customer"
    }
  },
  "errors": null,
  "timestamp": "2026-06-19T10:00:00.000Z"
}
```
 
Returns `401 Unauthorized` if credentials are invalid.
</details>

---

### User — `/api/user`
 
| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/api/user/profile` | Any authenticated user | Get the logged-in user's profile |
| PATCH | `/api/user/profile` | Any authenticated user | Update the logged-in user's name |
 
<details>
<summary><strong>GET /api/user/profile</strong></summary>

**Response** — `200 OK`:
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Profile fetched successfully",
  "data": {
    "id": 1,
    "name": "Jack Mathews",
    "email": "jack@example.com",
    "role": "Customer"
  },
  "errors": null,
  "timestamp": "2026-06-19T10:00:00.000Z"
}
```
</details>
<details>
<summary><strong>PATCH /api/user/profile</strong></summary>

**Request body** (`UserUpdateDTO`):
```json
{
  "name": "Jack M. Mathews"
}
```
 
**Response** — `200 OK`, returns the updated `UserDTO`.
</details>

---
 ### Book — `/api/book`
 
| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/api/book` | Any authenticated user | Get all books |
| GET | `/api/book/{id}` | Any authenticated user | Get a single book by ID |
| GET | `/api/book/search` | Any authenticated user | Search/filter books |
| POST | `/api/book` | Admin only | Create a new book |
| PUT | `/api/book/{id}` | Admin only | Update an existing book |
| DELETE | `/api/book/{id}` | Admin only | Delete a book |
 
<details>
<summary><strong>GET /api/book</strong></summary>

**Response** — `200 OK`:
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Books retrieved successfully",
  "data": [
    {
      "bookId": 1,
      "title": "Atomic Habits",
      "author": "James Clear",
      "genre": "Self Help",
      "description": null,
      "averageRating": 4.25
    }
  ],
  "errors": null,
  "timestamp": "2026-06-19T10:00:00.000Z"
}
```
 
`averageRating` is computed dynamically from all user ratings for that book — it is **not** a stored column. It is `null` if no one has rated the book yet.
</details>
<details>
<summary><strong>GET /api/book/{id}</strong></summary>

Returns a single `BookDTO`. Returns `400 Bad Request` if `id <= 0`, or `404 Not Found` if no book exists with that ID.
</details>
<details>
<summary><strong>GET /api/book/search</strong></summary>

**Query parameters** (`BookSearchDTO`, all optional):
 
| Param | Type | Matches against |
|---|---|---|
| `query` | string | Book title (partial, case-insensitive) |
| `author` | string | Author name (partial, case-insensitive) |
| `genre` | string | Genre (partial, case-insensitive) |
 
**Example:**
```
GET /api/book/search?query=dune&genre=sci-fi
```
 
Returns a list of `BookDTO`, same shape as `GET /api/book`.
</details>
<details>
<summary><strong>POST /api/book</strong> For Admin</summary>

**Request body** (`BookCreateDTO`):
```json
{
  "title": "Dune",
  "author": "Frank Herbert",
  "genre": "Science Fiction",
  "description": "Epic science fiction set in a desert world..."
}
```
 
**Response** — `201 Created`, returns the created `BookDTO`. Throws a duplicate-title error if a book with the same title already exists.
</details>
<details>
<summary><strong>PUT /api/book/{id}</strong> For Admin</summary>

**Request body** (`BookUpdateDTO`) — note `id` in the body must match the route `id`:
```json
{
  "id": 9,
  "title": "Dune",
  "author": "Frank Herbert",
  "genre": "Science Fiction",
  "description": "Updated description..."
}
```
 
**Response** — `200 OK`, returns the updated `BookDTO`. Returns `400 Bad Request` if the route and body IDs mismatch, `404 Not Found` if the book doesn't exist.
</details>
<details>
<summary><strong>DELETE /api/book/{id}</strong> For Admin</summary>

**Response** — `204 No Content` style response wrapped in the standard envelope. Returns `404 Not Found` if the book doesn't exist.
</details>

---
 ### User Book Activity — `/api/user`
 
> All endpoints below require role: **Customer**
 
| Method | Route | Description |
|---|---|---|
| POST | `/api/user/read` | Mark a book as read |
| POST | `/api/user/rate` | Rate a book (1–5) |
| GET | `/api/user/history` | Get the logged-in user's reading history |
 
<details>
<summary><strong>POST /api/user/read</strong></summary>

**Request body** (`UserReadDTO`):
```json
{
  "bookId": 9
}
```
 
**Response** — `200 OK`, confirms the book was marked as read for the current user.
</details>
<details>
<summary><strong>POST /api/user/rate</strong></summary>

**Request body** (`RateBookDTO`):
```json
{
  "bookId": 9,
  "rating": 4
}
```
 
`rating` must be between 1 and 5. Each user can have one rating per book — average ratings shown elsewhere in the app are computed across all users' ratings for that book.
 
**Response** — `200 OK`.
</details>
<details>
<summary><strong>GET /api/user/history</strong></summary>

**Response** — `200 OK`:
```json
{
  "success": true,
  "statusCode": 200,
  "message": "User history fetched successfully",
  "data": [
    {
      "bookId": 1,
      "title": "Atomic Habits",
      "author": "James Clear",
      "genre": "Self Help",
      "rating": 4,
      "isRead": true
    },
    {
      "bookId": 9,
      "title": "Dune",
      "author": "Frank Herbert",
      "genre": "Science Fiction",
      "rating": null,
      "isRead": false
    }
  ],
  "errors": null,
  "timestamp": "2026-06-19T10:00:00.000Z"
}
```
 
Returns one entry per book the user has either rated or marked as read. `rating` is `null` if the user hasn't rated that specific book.
</details>

---
### Recommendation — `/api/recommendation`
 
> Requires role: **Customer**
 
| Method | Route | Description |
|---|---|---|
| POST | `/api/recommendation` | Get hybrid book recommendations (internal + AI) |
 
<details>
<summary><strong>POST /api/recommendation</strong></summary>

**Request body** (`RecommendationRequestDTO`, all fields optional):
```json
{
  "prompt": "psychological thriller with a twist ending",
  "genre": "Thriller",
  "minimumRating": 4
}
```
 
**Response** — `200 OK`:
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Recommendations fetched successfully",
  "data": {
    "internalRecommendations": [
      {
        "bookId": 4,
        "title": "Gone Girl",
        "author": "Gillian Flynn",
        "genre": "Crime Thriller",
        "description": "...",
        "averageRating": 3.9
      }
    ],
    "externalRecommendations": [
      {
        "title": "The Silent Patient",
        "author": "Alex Michaelides",
        "genre": null,
        "isInternal": false,
        "searchUrl": "https://www.google.com/search?q=The%20Silent%20Patient%20book"
      }
    ]
  },
  "errors": null,
  "timestamp": "2026-06-19T10:00:00.000Z"
}
```
 
**How it works:**
1. `genre` and `minimumRating` filter the internal database — these matches become `internalRecommendations`.
2. The `prompt`, `genre`, and `minimumRating` are combined into a structured prompt sent to the Groq API (`llama-3.3-70b-versatile`).
3. The AI returns a list of suggested titles + authors.
4. Each AI suggestion is checked against the database — if it already exists internally, it's skipped (avoiding duplicates with `internalRecommendations`); if not, it's returned as an `externalRecommendations` entry with a generated Google search URL.
See the root [README](../README.md#ai-recommendation-engine-groq) for the full architecture diagram of this flow.
</details>

---
## Testing
 
The test project (`BookRecommendation.Tests`) uses **xUnit** for the test runner, **Moq** for mocking repositories/dependencies, and **FluentAssertions** for readable assertions.
 
**Run all tests:**
```bash
cd api/BookRecommendation.Tests
dotnet test
```
 
**Run with detailed output:**
```bash
dotnet test --verbosity normal
```
 
Tests are organized by service (e.g. `AuthServiceTests`, `BookServiceTests`) and mock the repository layer so business logic is tested in isolation from the database.
 
---
## Error Handling
 
The API uses a global exception-handling middleware (`Middlewares/`) so controllers stay clean — most controller actions don't need try/catch blocks. Unhandled exceptions are caught centrally and returned in the standard `ApiResponse` envelope with an appropriate status code.
 
---
 
## Related Documentation
 
- [Root README](../README.md) — project overview, features, system architecture
- [Frontend README](../ui/README.md) — Angular setup and component documentation