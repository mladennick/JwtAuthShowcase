# JWT Authentication Showcase

This repository is a proof-of-concept and portfolio project demonstrating secure, decoupled JSON Web Token (JWT) authentication in .NET, with a reusable core library and a small MVC demo app.

## Architecture

The solution currently has three projects:

- **`JwtAuth.Core`**: reusable JWT class library (token generation, settings model, and DI extension).
- **`JwtAuth.Core.Tests`**: xUnit test suite focused on token correctness and security behavior.
- **`Demo.Mvc`**: ASP.NET Core MVC Expense Tracker demo with register/login flow, token issuance, and persisted user sessions.

## Current Demo Scope

`Demo.Mvc` includes:

- User registration and login.
- JWT generation through `JwtAuth.Core`.
- `UserSessions` table storing issued token sessions (`jti`, timestamps, revoked flag).
- Session revocation from the UI.
- JWT validation hook that rejects revoked/unknown/expired sessions.
- Protected sample endpoint at `GET /api/me`.

## Data Provider

The demo app currently uses **SQLite** (not PostgreSQL):

- Provider: `Microsoft.EntityFrameworkCore.Sqlite`
- Connection string: `Data Source=expense-tracker.db`
- Database creation: automatic on startup (`EnsureCreated`)

## Technologies Used

- C# / .NET 9
- ASP.NET Core MVC
- JWT Bearer Authentication
- Entity Framework Core
- SQLite
- xUnit

## Test Coverage (Core Library)

The `JwtAuth.Core` test suite covers:

- token generation and expected claims
- settings validation and fail-fast configuration behavior
- expiry boundary checks
- signature validation success/failure paths
- wrong-key rejection
- payload tampering rejection

See `JwtAuth.Core.Tests/README.md` for details.

## Running the Demo

### Prerequisites

- .NET SDK 9+

### Start

1. Clone the repository.
2. From repository root, run:
   - `dotnet run --project Demo.Mvc`
3. Open the app URL shown in terminal.
4. Use:
   - `/Auth/Register` to create a user
   - `/Auth/Login` to issue a token
   - `/Auth/Sessions` to inspect/revoke sessions
   - `/api/me` with Bearer token to test protected access

## Next Steps

- Add logout endpoint that revokes the currently active token.
- Add expense persistence and user-scoped data access.
- Optionally switch demo storage from SQLite to PostgreSQL (`Npgsql`).