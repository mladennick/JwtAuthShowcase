# JWT Authentication & PostgreSQL Showcase

This repository is a proof-of-concept and portfolio project that demonstrates secure, decoupled JSON Web Token (JWT) authentication in a .NET Web API, together with PostgreSQL integration through Entity Framework Core.

## Architecture

The solution is split into two components to keep concerns clearly separated:

* **`JwtAuth.Core`**: A reusable class library that contains JWT generation logic, configuration models, and dependency injection extensions. It is designed to be dropped into other .NET projects.
* **`Demo.Api`**: An ASP.NET Core Web API that consumes the core authentication library and exposes secured endpoints (`[Authorize]`) backed by PostgreSQL.

## Technologies Used

* C# / .NET
* ASP.NET Core Web API
* Entity Framework Core
* PostgreSQL (`Npgsql`)
* JWT Bearer Authentication
* Swagger / OpenAPI (Configured for Bearer Token input)

## Test Coverage (Core Library)

The core JWT class library has an xUnit test suite covering token creation, settings validation, expiration boundaries, signature verification, wrong-key rejection, and payload tampering rejection.

See `JwtAuth.Core.Tests/README.md` for details.

## Project Status / Roadmap

- `JwtAuth.Core`: implemented and covered by unit tests.
- `JwtAuth.Core.Tests`: active and expanding security-focused coverage.
- `Demo.Api`: planned next step (integration, login flow, and secured endpoints).

## Getting Started

*(These instructions will be expanded once `Demo.Api` and database setup are fully configured.)*

### Prerequisites
* .NET SDK installed
* PostgreSQL server running locally or via Docker

### Running the App
1. Clone the repository.
2. Update the `appsettings.json` in the `Demo.Api` project with your PostgreSQL connection string.
3. Apply database migrations: `dotnet ef database update`
4. Run the API project.
5. Navigate to the Swagger UI to test the `/login` endpoint and access the secured PostgreSQL data endpoints.