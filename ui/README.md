#  Frontend — AI Book Recommendation System (ui/)
 
Angular application powering the customer and admin interfaces for the Book Recommendation System. Built with standalone components, role-based routing, and styled using TailwindCSS + DaisyUI.
 
---
## Tech Stack
 
| Component | Technology |
|---|---|
| Framework | Angular 19 (standalone components) |
| Styling | TailwindCSS, DaisyUI |
| HTTP | Angular `HttpClient` with functional interceptors |
| Routing | Angular Router with functional guards |
| State | Component-level state + `localStorage` for auth session |
 
---
## Project Structure
 
```
ui/
├── src/app/
│   ├── core/
│   │   ├── guards/            # Route guards (auth, admin, customer)
│   │   ├── interceptors/      # HTTP interceptors (JWT attachment)
│   │   ├── models/
│   │   │   ├── auth/           # User, login/register, ApiResponse
│   │   │   ├── books/          # Book, CreateBook, UpdateBook, BookSearch, UserHistory, UserRead, RateBook
│   │   │   └── recommendation/ # RecommendationRequest/Response, RecommendationBook
│   │   └── services/          # AuthService, BookService, UserService, RecommendationService
│   │
│   ├── shared/                 # Reusable components (navbar, sidebar, etc.)
│   │
│   ├── features/
│   │   ├── home/                # Public landing page
│   │   ├── auth/
│   │   │   ├── login/
│   │   │   └── register/
│   │   ├── admin/
│   │   │   ├── admin-layout/     # Shell with navbar + sidebar for admin routes
│   │   │   ├── admin-dashboard/
│   │   │   └── admin-books/      # Book CRUD management
│   │   └── customer/
│   │       ├── customer-layout/  # Shell with navbar + sidebar for customer routes
│   │       ├── customer-dashboard/
│   │       ├── customer-books/   # Book browsing, search, mark-read, rate
│   │       └── recommendation/   # AI recommendation page
│   │
│   ├── app.component.ts
│   ├── app.config.ts            # App-wide providers (router, HttpClient, interceptors)
│   └── app.routes.ts            # Route definitions + guards
│
├── environments/                # API URL configuration per environment
├── proxy.conf.json              # Dev-time proxy to backend API
├── angular.json
├── package.json
└── tailwind.config / postcss config
```
 
---
## Setup & Run
 
### Prerequisites
- Node.js (v18+)
- Angular CLI
- The backend API running locally (see [api/README.md](../api/README.md))
### Install dependencies
```bash
cd ui
npm install
```
 
### Run the dev server
```bash
ng serve
```
The app runs on `http://localhost:4200` by default. API calls made to `/api/...` are automatically proxied to the backend via `proxy.conf.json` — **make sure the backend is running first**, otherwise requests will fail.
 
---
 
## Dev Proxy Configuration
 
`proxy.conf.json` forwards all `/api` requests from the Angular dev server to the backend:
 
```json
{
  "/api": {
    "target": "https://localhost:7290",
    "secure": false,
    "changeOrigin": true
  }
}
```
 
If your backend runs on a different port, update the `target` value accordingly. `secure: false` allows the proxy to accept the backend's local self-signed HTTPS certificate during development.
 
---
## Authentication Flow
 
- On login, the backend returns a JWT and a user object. Both are stored in `localStorage` via `AuthService`.
- A functional **HTTP interceptor** (`authInterceptor`) automatically attaches the token to every outgoing request:
```typescript
  Authorization: Bearer <token>
```
- `AuthService.getRole()` reads the role (`Customer` / `Admin`) straight from the stored user object — this drives both route guards and conditional UI (navbar badge, sidebar links).
- Logout simply clears `token` and `user` from `localStorage`.
> Auth state is intentionally kept simple (`localStorage`-based) since this is a training project — no refresh-token flow or token expiry handling is implemented.
 
---
 
## Route Guards
 
Three functional guards control access, applied via `canActivate` in `app.routes.ts`:
 
| Guard | Checks | Redirects to (if failed) |
|---|---|---|
| `authGuard` | User is logged in (`isLoggedIn()`) | `/login` |
| `adminGuard` | Role is `Admin` | `/` |
| `customerGuard` | Role is `Customer` | `/` |
 
Admin and customer route groups apply **both** `authGuard` and their respective role guard:
 
```typescript
{
  path: "admin",
  component: AdminLayoutComponent,
  canActivate: [authGuard, adminGuard],
  children: [ ... ]
}
```
 
This means an unauthenticated user is bounced to `/login`, while a logged-in customer trying to access `/admin/*` is bounced to `/` rather than `/login` — they're authenticated, just not authorized for that section.
 
---
 
## Routes
 
| Path | Component | Guards | Description |
|---|---|---|---|
| `/` | `HomeComponent` | None | Public landing page |
| `/login` | `LoginComponent` | None | Login (both roles) |
| `/register` | `RegisterComponent` | None | Customer registration |
| `/admin/dashboard` | `AdminDashboardComponent` | `authGuard`, `adminGuard` | Admin profile + quick actions |
| `/admin/books` | `AdminBooksComponent` | `authGuard`, `adminGuard` | Book CRUD management |
| `/customer/dashboard` | `CustomerDashboardComponent` | `authGuard`, `customerGuard` | Customer profile + reading history |
| `/customer/books` | `CustomerBooksComponent` | `authGuard`, `customerGuard` | Browse, search, mark-read, rate books |
| `/customer/recommendation` | `RecommendationComponent` | `authGuard`, `customerGuard` | AI-powered book recommendations |
 
Admin and customer routes are nested under their respective `*-layout` components (`AdminLayoutComponent`, `CustomerLayoutComponent`), which render the shared navbar + sidebar shell and an inner `<router-outlet>` for the child page.
 
---
 
## Core Services
 
All services live in `core/services/` and are provided at the root level (`providedIn: 'root'`).
 
### `AuthService`
| Method | Description |
|---|---|
| `register(data: RegisterRequest)` | Registers a new customer |
| `login(data: LoginRequest)` | Logs in, stores token + user in `localStorage` |
| `logout()` | Clears stored session |
| `getToken()` | Returns the stored JWT, or `null` |
| `isLoggedIn()` | Returns `true` if a token is present |
| `getUser()` | Returns the stored `User` object, or `null` |
| `getRole()` | Returns the user's role string, or `null` |
 
### `BookService`
| Method | Description |
|---|---|
| `getBooks()` | Fetches all books |
| `createBook(data: CreateBook)` | Admin — creates a new book |
| `updateBook(id, data: UpdateBook)` | Admin — updates a book |
| `deleteBook(id)` | Admin — deletes a book |
| `searchBooks(params: BookSearch)` | Searches/filters by `query`, `author`, `genre` (builds query string dynamically) |
 
### `UserService`
| Method | Description |
|---|---|
| `getProfile()` | Gets the logged-in user's profile |
| `updateProfile(data: UserUpdate)` | Updates the user's name |
| `getHistory()` | Gets the customer's reading history |
| `markAsRead(data: UserRead)` | Marks a book as read |
| `rateBook(data: RateBook)` | Submits a rating (1–5) for a book |
 
### `RecommendationService`
| Method | Description |
|---|---|
| `getRecommendation(data: RecommendationRequest)` | Sends prompt/genre/minimumRating and gets back combined internal + external recommendations |
 
---
 
## Models
 
All API request/response shapes are typed under `core/models/`, organized by domain.
 
**`models/auth/`** — `ApiResponse<T>`, `User`, `LoginRequest`, `RegisterRequest`, `LoginResponse`, `UserUpdate`
 
**`models/books/`** — `Book`, `CreateBook`, `UpdateBook`, `BookSearch`, `UserHistory`, `UserRead`, `RateBook`
 
**`models/recommendation/`** — `RecommendationRequest`, `RecommendationBook`, `RecommendationResponse`
 
| Model | Key fields |
|---|---|
| `Book` | `bookId`, `title`, `author`, `genre?`, `description?`, `averageRating?` |
| `RecommendationResponse` | `internalRecommendations: Book[]`, `externalRecommendations: RecommendationBook[]` |
| `UserHistory` | `bookId`, `title`, `author`, `genre`, `rating`, `isRead` |
 
> `averageRating` on `Book` is never sent by the client — it's computed dynamically by the backend and only appears in responses.
 
---
 
## Shared Components
 
| Component | Used by | Purpose |
|---|---|---|
| Navbar | Both layouts | Shows logo, role badge, avatar initials, logout button — content adapts based on `AuthService.getRole()` |
| Sidebar | Both layouts | Role-specific navigation links (different items for Admin vs Customer) |
 
Both layouts (`AdminLayoutComponent`, `CustomerLayoutComponent`) wrap these shared components around a `<router-outlet>`, so individual feature pages only need to render their own content.
 
---
 
## Key Dependencies
 
| Package | Version |
|---|---|
| `@angular/core` | ^19.2.0 |
| `@angular/router` | ^19.2.0 |
| `@angular/common` | ^19.2.0 |
| `tailwindcss` | ^4.3.0 |
| `daisyui` | ^5.5.20 |
| `rxjs` | ^7.8.0 |
| `typescript` (dev) | ~5.7.2 |
 
See `package.json` for the full dependency list.
 
---
 
## Related Documentation
 
- [Root README](../README.md) — project overview, features, system architecture
- [Backend README](../api/README.md) — API endpoints, auth, configuration setup
 