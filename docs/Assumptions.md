# Assumptions & Limitations
 
This document records deliberate design decisions, scope boundaries, and known limitations made during development — so anyone reviewing the project understands *why* certain things work the way they do, rather than assuming they're oversights.
 
---
## Authentication & Authorization
 
- **No admin self-registration.** The `/api/auth/register` endpoint always assigns the `Customer` role. Admin accounts must be created directly in the database. This was a deliberate security decision — exposing an admin-registration endpoint publicly would be a significant risk for a system like this.
- **Authorization is enforced at the backend, by design.** Frontend route guards (`authGuard`, `adminGuard`, `customerGuard`) only control navigation/UX — the real security boundary is the backend's `[Authorize(Roles=...)]` attributes, since a route guard can't stop someone from calling the API directly. This separation was intentional, not an oversight.
---
## Ratings & Reviews
 
- **`averageRating` is computed dynamically, not stored.** There is no `AverageRating` column on the `Books` table — it's calculated on every read by aggregating the related `UserBooks` rows. This guarantees the value is always accurate without needing a recalculation step on every new rating, at the cost of a slightly more expensive query.
- **One rating per user per book.** A user can update their existing rating but cannot submit multiple separate ratings for the same book — this is enforced by `UserBooks` being a single row per `(UserId, BookId)` pair.
- **Admins cannot rate books.** This is enforced via the `[Authorize(Roles = "Customer")]` attribute on `UserBookController`, not by any UI-only restriction.
---
## Recommendation Engine (Groq AI)
 
- **AI provider was switched from Gemini to Groq mid-development**, after encountering model deprecation (`404`) and free-tier rate limiting (`429`) issues with Gemini. Groq's free tier proved more stable for iterative development.
- **AI response parsing is format-dependent.** The system expects Groq to return lines in a `Title | Author` format, as instructed in the prompt. If the model deviates from this format (which LLMs occasionally do despite instructions), parsing may produce incomplete or malformed entries. There's no schema validation or retry-on-malformed-response logic.
- **No caching of recommendation results.** Every request re-queries the database and re-calls Groq, even for identical or near-identical prompts. This is acceptable for a training project's traffic volume but would not scale well as-is.
---
## Database
 
- **Identity (auto-increment) IDs are not guaranteed to be sequential from .** SQL Server's identity counter does not reset or reuse values after deletions, so `BookId` values may have gaps (e.g. jumping from 9 to 1002) after testing/seeding activity. This is standard SQL Server behavior, not an application bug, and has no functional impact since IDs are used purely as keys, never displayed to end users.
---
## Configuration & Secrets
 
- **`appsettings.json` is committed with placeholder values only.** Real secrets (connection string, JWT secret, Groq API key) must be supplied locally via `appsettings.Development.json`, which is gitignored.
---
 
## Testing
 
- **Test coverage is concentrated on `AuthService` and `BookService`.** `UserBookService` and `RecommendationService` have lighter or no coverage — the latter's dependency on an external HTTP call makes it harder to mock cleanly within current scope.
---
 
## Deployment & Scalability
 
- **Built and documented for local development only** — `ng serve` + `dotnet run` against a local SQL Server instance. No containerization, CI/CD, or cloud deployment is currently configured.
---
 
## Summary
 
Most limitations above stem from intentionally scoping this as a **training project** rather than a production system — the priority was demonstrating correct architecture, working end-to-end features, and a functioning AI integration, rather than building out every production-readiness concern (caching, retries, pagination, CI/CD, etc.). Where a limitation exists, it's noted above along with what a more complete implementation would add.
 