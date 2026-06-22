# Architecture Overview
 
## 1. System Type
 
The AI Book Recommendation System is a **three-tier client-server application**:
 
```mermaid
flowchart LR
    A[Presentation Tier<br/>Angular SPA] <--> B[Application Tier<br/>ASP.NET Core Web API]
    B <--> C[Data Tier<br/>SQL Server]
    B -- "HTTPS, structured prompt" --> D[Groq API External<br/>llama-3.3-70b-versatile]
    D -- "AI-generated suggestions" --> B
 
    style A fill:#2d6a4f,color:#fff
    style B fill:#1a1714,color:#fff
    style C fill:#e67e22,color:#fff
    style D fill:#8e44ad,color:#fff
```
 
The frontend never talks to Groq directly — all AI calls are proxied through the backend, so the API key is never exposed to the client.
 
---
## 2. Layered Backend Architecture
 
The backend follows a strict **Repository → Service → Controller** pattern, with a separate DTO layer for all data crossing the API boundary.
 
```mermaid
flowchart TD
    A[HTTP Request] --> B
 
    subgraph B[Controller Layer]
        B1[Route handling, <br/>model binding]
        B2["[Authorize] role checks"]
        B3[Maps domain models ↔ <br/>DTOs via AutoMapper]
    end
 
    B --> C
 
    subgraph C[Service Layer]
        C1[Business rules - <br/>duplicate checks, <br/>validation]
        C2[Orchestrates repository <br/>calls]
        C3[Calls external services - <br/>Groq]
    end
 
    C --> D
 
    subgraph D[Repository Layer]
        D1[EF Core queries only]
        D2[No business logic]
    end
 
    D --> E[SQL Server<br/>via DbContext]
 
    style B fill:#1a1714,color:#fff
    style C fill:#2d6a4f,color:#fff
    style D fill:#1a1714,color:#fff
    style E fill:#e67e22,color:#fff
```
 
**Why this separation:**
- Controllers stay thin — they only handle HTTP concerns.
- Services hold all business rules in one place, independent of how data is fetched or how the result will be transported.
- Repositories can be mocked in unit tests (via interfaces), letting service logic be tested without touching a real database.
- DTOs prevent leaking EF Core entity internals (e.g. navigation properties, tracking metadata) to the client.
A global exception-handling **middleware** sits in front of the controller pipeline, so most controller actions don't need explicit try/catch blocks — unhandled exceptions are caught centrally and returned in the standard `ApiResponse` envelope.
 
---
## 3. Frontend Architecture
 
The Angular app uses **standalone components**  organized by responsibility:
 
```mermaid
flowchart TD
    A[src/app/] --> B[core/<br/>App-wide singleton concerns<br/>auth, http, models, guards]
    A --> C[shared/<br/>Reusable UI building blocks<br/>used by multiple features]
    A --> D[features/<br/>Page-level components<br/>grouped by domain]
 
    style A fill:#1a1714,color:#fff
    style B fill:#2d6a4f,color:#fff
    style C fill:#e67e22,color:#fff
    style D fill:#8e44ad,color:#fff
```
 
**Routing & access control** is handled by functional guards (`authGuard`, `adminGuard`, `customerGuard`) composed at the route definition level — not inside components. This keeps authorization logic declarative and centralized in `app.routes.ts` rather than scattered across component constructors.
 
**Cross-cutting HTTP concerns** (attaching the JWT) are handled by a functional **interceptor**, registered once in `app.config.ts`, rather than manually added to every service method.
 
---
## 4. Authentication & Authorization Architecture
 
```mermaid
flowchart TD
    A[Client] -- credentials --> B[Auth API]
    B --> C[Verifies password - BCrypt<br/>Signs JWT - HMAC-SHA256]
    C --> D[JWT issued<br/>+ Role claim]
    D --> E[Stored in localStorage<br/>on client]
    E --> F["Every subsequent request →<br/>Authorization: Bearer <br/>token"]
    F --> G[ASP.NET Core middleware<br/>validates signature + <br/>expiry]
    G --> H["[Authorize(Roles = ...)]<br/>checks Role claim <br/>per-endpoint"]
 
    style A fill:#2d6a4f,color:#fff
    style B fill:#1a1714,color:#fff
    style D fill:#e67e22,color:#fff
    style G fill:#1a1714,color:#fff
    style H fill:#8e44ad,color:#fff
```
 
Role enforcement happens at **two independent layers**:
1. **Frontend route guards** — prevent navigating to a restricted page (UX-level gatekeeping).
2. **Backend `[Authorize(Roles = ...)]` attributes** — the actual security boundary; the frontend guard alone is not sufficient since a user could call the API directly.
This dual-layer approach means the frontend guard is purely for user experience (e.g. don't even render a page you'll get rejected from), while the backend attribute is the system's real authorization boundary.
 
---

## 5. Recommendation Engine Architecture (Hybrid AI)
 
The recommendation feature is the most architecturally distinct part of the system, since it's the only flow that integrates a synchronous external API call into the request lifecycle.
 
```mermaid
flowchart TD
    A["Customer submits:<br/>{ prompt, genre?, <br/>minimumRating? }"] --> B
 
    subgraph B[Stage 1 — Internal Filter]
        B1[Query Books table by genre/rating]
        B2["→ internalRecommendations[]"]
    end
 
    B --> C
 
    subgraph C[Stage 2 — AI Suggestion]
        C1[Build structured prompt]
        C2[Call Groq chat completions API]
        C3["Parse 'Title | Author' lines"]
        C4["→ externalRecommendations[]<br/>+ generated Google search URL"]
    end
 
    C --> E["Response:<br/>{ internalRecommendations[], <br/> externalRecommendations[] }"]
 
    style B fill:#2d6a4f,color:#fff
    style C fill:#e67e22,color:#fff
```
 
This stage separation means a failure in Stage 2 (e.g. Groq API timeout or rate limit) currently fails the entire request — see [Assumptions & Limitations](./assumptions.md) for known constraints around this.

---

 ## 6. Data Model Relationships
 
```mermaid
erDiagram
    USERS {
        int Id
        string Name
        string Email
        string Password
        string Role
    }
    USERBOOKS {
        int Id
        int UserId
        int BookId
        double Rating
        bool IsRead
    }
    BOOKS {
        int BookId
        string Title
        string Author
        string Genre
        string Description
    }
 
    USERS ||--o{ USERBOOKS : "has many"
    BOOKS ||--o{ USERBOOKS : "has many"
```
 
`UserBooks` is a join entity capturing **per-user, per-book** state — both the read status and rating live here, not on the `Books` table directly. This is what allows `averageRating` to be computed dynamically (aggregating across all `UserBooks` rows for a given `BookId`) rather than stored as a static column that would need manual recalculation on every new rating.
 
---
## 7. Deployment Topology (Current — Local Development)
 
```mermaid
flowchart TD
    subgraph Local[Developer Machine]
        A[Angular dev server<br/>http://localhost:4200]
        B[.NET API<br/>https://localhost:7290]
        C[SQL Server - local<br/>Trusted Connection]
    end
 
    D[Groq API<br/>https://api.groq.com<br/>HTTPS, API key auth]
 
    A -- "/api/* via proxy.conf.json" --> B
    B --> C
    B -- HTTPS --> D
 
    style A fill:#2d6a4f,color:#fff
    style B fill:#1a1714,color:#fff
    style C fill:#e67e22,color:#fff
    style D fill:#8e44ad,color:#fff
```
 
The Angular dev server proxies `/api/*` requests to the local backend via `proxy.conf.json`, simulating same-origin behavior during development. No containerization or cloud deployment is currently configured — see [Assumptions & Limitations](./assumptions.md).

 