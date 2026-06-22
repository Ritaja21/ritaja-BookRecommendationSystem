# Data Flow & Sequence Diagrams

Detailed request-level flows for every major feature. For the high-level system architecture, see [architecture.md](./architecture.md).

---

## 1. Registration & Login

```mermaid
sequenceDiagram
    actor User
    participant UI as Angular UI
    participant API as AuthController
    participant Svc as AuthService
    participant Repo as AuthRepository
    participant DB as SQL Server

    User->>UI: Submit registration form
    UI->>API: POST /api/auth/register
    API->>Svc: RegisterAsync(dto)
    Svc->>Repo: IsEmailExistsAsync(email)
    Repo->>DB: SELECT WHERE Email = ...
    DB-->>Repo: result
    Repo-->>Svc: bool

    alt Email already exists
        Svc-->>API: throws InvalidOperationException
        API-->>UI: 400 Bad Request
    else Email available
        Svc->>Svc: Hash password (BCrypt)
        Svc->>Repo: RegisterAsync(user)
        Repo->>DB: INSERT INTO Users
        DB-->>Repo: created user
        Repo-->>Svc: User entity
        Svc-->>API: UserDTO
        API-->>UI: 201 Created
    end

    User->>UI: Submit login form
    UI->>API: POST /api/auth/login
    API->>Svc: LoginAsync(dto)
    Svc->>Repo: GetUserByEmailAsync(email)
    Repo->>DB: SELECT WHERE Email = ...
    DB-->>Repo: User entity or null

    alt User not found or password invalid
        Svc-->>API: null
        API-->>UI: 401 Unauthorized
    else Credentials valid
        Svc->>Svc: Generate JWT (claims: Id, Name, Email, Role)
        Svc-->>API: LoginResponseDTO { token, userDTO }
        API-->>UI: 200 OK
        UI->>UI: Store token + user in localStorage
        UI->>UI: Redirect based on role
    end
```

---

## 2. Authenticated Request Flow (Generic)

Applies to every protected endpoint after login.

```mermaid
sequenceDiagram
    actor User
    participant UI as Angular Component
    participant Interceptor as authInterceptor
    participant Guard as Route Guard
    participant API as Controller
    participant MW as Auth Middleware

    User->>UI: Navigate to protected route
    UI->>Guard: canActivate()
    Guard->>Guard: authService.isLoggedIn() / getRole()

    alt Not logged in
        Guard-->>UI: redirect to /login
    else Wrong role
        Guard-->>UI: redirect to /
    else Authorized
        Guard-->>UI: allow navigation
        UI->>Interceptor: HTTP request
        Interceptor->>Interceptor: Attach Authorization: Bearer <token>
        Interceptor->>API: Forward request
        API->>MW: Validate JWT signature + expiry
        MW->>MW: Check [Authorize(Roles=...)] against token claim

        alt Token invalid/expired or role mismatch
            MW-->>API: 401 Unauthorized
        else Valid
            API->>API: Process request normally
            API-->>UI: 200 OK + data
        end
    end
```

---

## 3. Book Browsing & Search (Customer)

```mermaid
sequenceDiagram
    actor Customer
    participant UI as CustomerBooksComponent
    participant API as BookController
    participant Svc as BookService
    participant Repo as BookRepository
    participant DB as SQL Server

    Customer->>UI: Open Browse Books page
    UI->>API: GET /api/book
    API->>Svc: GetBooksAsync()
    Svc->>Repo: GetBooksAsync()
    Repo->>DB: SELECT * FROM Books (with UserBooks for rating calc)
    DB-->>Repo: Book entities
    Repo-->>Svc: List<Book>
    Svc-->>API: List<Book>
    API->>API: Map to BookDTO (computes averageRating per book)
    API-->>UI: 200 OK + BookDTO[]
    UI->>UI: Render book grid

    Customer->>UI: Type in search box / genre filter
    UI->>API: GET /api/book/search?query=...&genre=...
    API->>Svc: SearchBooksAsync(searchDTO)
    Svc->>Repo: SearchBooksAsync(searchDTO)
    Repo->>DB: SELECT WHERE Title/Author/Genre LIKE ...
    DB-->>Repo: filtered Book entities
    Repo-->>Svc: List<Book>
    Svc-->>API: List<Book>
    API-->>UI: 200 OK + filtered BookDTO[]
    UI->>UI: Re-render grid with results
```

---

## 4. Mark as Read & Rate Book

```mermaid
sequenceDiagram
    actor Customer
    participant UI as CustomerBooksComponent
    participant API as UserBookController
    participant Svc as UserBookService
    participant Repo as UserBookRepository
    participant DB as SQL Server

    Customer->>UI: Click "Mark Read"
    UI->>API: POST /api/user/read { bookId }
    API->>API: Extract userId from JWT claims
    API->>Svc: MarkBookAsReadAsync(userId, dto)
    Svc->>Repo: Upsert UserBook { userId, bookId, isRead: true }
    Repo->>DB: INSERT or UPDATE UserBooks
    DB-->>Repo: ack
    Repo-->>Svc: result
    Svc-->>API: result
    API-->>UI: 200 OK

    Customer->>UI: Click "Rate" → select stars → submit
    UI->>API: POST /api/user/rate { bookId, rating }
    API->>API: Extract userId from JWT claims
    API->>Svc: RateBookAsync(userId, dto)
    Svc->>Repo: Upsert UserBook { userId, bookId, rating }
    Repo->>DB: INSERT or UPDATE UserBooks
    DB-->>Repo: ack
    Repo-->>Svc: result
    Svc-->>API: result
    API-->>UI: 200 OK
    UI->>UI: Optionally refresh book list to show new average rating
```

---

## 5. AI Recommendation Flow (Detailed)

This is the most complex flow in the system — the only one involving a synchronous external API call mid-request.

```mermaid
sequenceDiagram
    actor Customer
    participant UI as RecommendationComponent
    participant API as RecommendationController
    participant Svc as RecommendationService
    participant DB as SQL Server
    participant Groq as Groq API

    Customer->>UI: Enter prompt + optional genre/minRating
    UI->>API: POST /api/recommendation
    API->>Svc: GetRecommendationAsync(requestDTO)

    rect rgb(7, 39, 23)
    Note over Svc,DB: Stage 1 — Internal DB Filter
    Svc->>DB: Query Books WHERE Genre LIKE ... 
    DB-->>Svc: matching Book entities
    Svc->>Svc: Filter further by avg rating ≥ minimumRating
    Svc->>Svc: internalRecommendations = mapped BookDTO[]
    end

    rect rgb(157, 82, 13)
    Note over Svc,Groq: Stage 2 — AI Suggestion
    Svc->>Svc: Build structured prompt (genre + user interest)
    Svc->>Groq: POST /openai/v1/chat/completions
    Groq-->>Svc: { choices: [{ message: { content: "Title | Author\n..." } }] }
    Svc->>Svc: Split response into lines, parse "Title | Author"
    end

    rect rgb(109, 9, 148)
    Note over Svc,DB: Stage 3 — Deduplication
    loop for each AI-suggested book
        Svc->>Svc: Check if title exists in already-fetched DB books
        alt Exists internally
            Svc->>Svc: Skip (already covered by internalRecommendations)
        else Doesn't exist
            Svc->>Svc: Generate Google search URL
            Svc->>Svc: Add to externalRecommendations[]
        end
    end
    end

    Svc-->>API: RecommendationResponseDTO
    API-->>UI: 200 OK { internalRecommendations, externalRecommendations }
    UI->>UI: Render two-column result view
```

---

## 6. Admin Book Management (CRUD)

```mermaid
sequenceDiagram
    actor Admin
    participant UI as AdminBooksComponent
    participant API as BookController
    participant Svc as BookService
    participant Repo as BookRepository
    participant DB as SQL Server

    Admin->>UI: Fill "Add Book" form → Save
    UI->>API: POST /api/book
    API->>API: [Authorize(Roles="Admin")] check
    API->>Svc: CreateBookAsync(dto)
    Svc->>Repo: GetBookByNameAsync(title)
    Repo->>DB: SELECT WHERE Title = ...
    DB-->>Repo: existing book or null

    alt Duplicate title found
        Svc-->>API: throws Exception
        API-->>UI: 400/500 error
    else No duplicate
        Svc->>Repo: CreateBookAsync(book)
        Repo->>DB: INSERT INTO Books
        DB-->>Repo: created Book
        Repo-->>Svc: Book entity
        Svc-->>API: Book entity
        API-->>UI: 201 Created + BookDTO
        UI->>UI: Refresh book list
    end

    Admin->>UI: Click "Delete" on a book
    UI->>API: DELETE /api/book/{id}
    API->>Svc: DeleteBookAsync(id)
    Svc->>Repo: DeleteBookAsync(id)
    Repo->>DB: DELETE FROM Books WHERE BookId = id
    DB-->>Repo: rows affected
    Repo-->>Svc: bool
    alt Book not found
        Svc-->>API: false
        API-->>UI: 404 Not Found
    else Deleted
        Svc-->>API: true
        API-->>UI: 200 OK
        UI->>UI: Remove from list
    end
```

---

## 7. Average Rating Computation (Conceptual Flow)

Not a request flow, but worth documenting since it's a recurring computed value across multiple endpoints (`GET /api/book`, `GET /api/book/{id}`, search, recommendations).

```mermaid
flowchart TD
    A[Book requested] --> B{Any UserBooks rows<br/>for this BookId with<br/>non-null Rating?}
    B -- No --> C[averageRating = null]
    B -- Yes, exactly one --> D[averageRating = that single rating]
    B -- Yes, multiple --> E[averageRating = AVG of all ratings]
    C --> F[Frontend shows<br/>'No ratings yet']
    D --> G[Frontend shows<br/>that rating as stars]
    E --> G
```

This computation happens **every time** a book is fetched — there is no cached or stored `AverageRating` column on the `Books` table, by design (see [Assumptions & Limitations](./assumptions.md)).