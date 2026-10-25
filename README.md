# Runner Car Rentals

A full-stack car rental management platform built with **Angular 19**, **ASP.NET Core 8 Web API**, **Entity Framework Core** and **SQL Server**. The project is structured around Clean Architecture and implements a complete booking lifecycle: browse the fleet, check real availability for a date range, request a booking, and manage it as a customer or as an admin.

## Overview

Runner Car Rentals is a portfolio project focused on production-style backend design:

- Business rules (availability, pricing, cancellation) live in the **Application** layer, independent of the database and the UI.
- Persistence is implemented with **EF Core repositories** and a unit of work.
- The API uses **JWT authentication**, **role-based authorization**, **ProblemDetails error responses** and **Swagger**.
- The Angular client is built with **standalone components**, **signals**, **lazy-loaded routes**, a functional HTTP interceptor and route guards.

## Features

### Authentication & accounts
- Register and log in with email + password.
- Passwords are hashed with **PBKDF2-SHA512** (100 000 iterations, per-password salt) — plaintext passwords are never stored.
- JWT access tokens include the user id, name, email and role.
- Role-based access control with `Customer` and `Admin` roles.
- Angular auth service (signal-based session), bearer-token interceptor and route guards.

### Car catalog
- Public catalog of vehicles with make, model, year, plate, seats, transmission, fuel type, daily rate and image.
- Text search across make, model and license plate.
- **Date-range availability filter**: cars with an overlapping active booking are excluded.
- Vehicle detail page with specs and a booking panel.

### Booking engine
- Create a booking for a date range; the server computes the total from the daily rate and rental days.
- **Overlap prevention** at the database query level — no double bookings for the same car.
- Validations: no past pick-up dates, drop-off must be after pick-up, maximum 90-day rentals.
- Booking lifecycle: `Pending → Confirmed → Completed`, plus `Cancelled`.
- Customers can view their bookings and cancel upcoming ones; admins can cancel any active booking.
- Availability endpoint returns whether a car is free for a range and the estimated total.

### Admin dashboard
- Add, edit and delete vehicles.
- Unique license plate enforcement; cars with booking history cannot be deleted (mark them unavailable instead).
- View every booking with customer details and change its status.

### Platform
- Clean Architecture layering with dependencies pointing inward.
- EF Core code-first schema, migrations and seed data (admin user + sample fleet) on startup.
- Centralised exception handling that maps domain/application errors to correct HTTP status codes.
- CORS restricted to the Angular dev origin; Swagger UI with bearer-token support.

## Tech stack

| Area | Technology |
| --- | --- |
| Frontend | Angular 19 (standalone components, signals, lazy routes), TypeScript, Bootstrap 5, Angular Material |
| Backend | ASP.NET Core 8 Web API, C# 12 |
| Persistence | Entity Framework Core 8, SQL Server |
| Auth | JWT bearer tokens, PBKDF2 password hashing, role-based authorization |
| API docs | Swagger / OpenAPI |
| Architecture | Clean Architecture, Repository pattern, Unit of Work, SOLID |

## Architecture

```
back-end.Api             → controllers, middleware, composition root
back-end.Contracts       → request/response DTOs shared by the API and client
back-end.Application     → business rules, service interfaces, abstractions
back-end.Domain          → entities and enums (no framework dependencies)
back-end.Infrastructure  → EF Core DbContext, repositories, JWT, password hashing
```

Dependency flow:

```
Api ──► Application ──► Domain
 │            ▲
 └──► Infrastructure ──► Application ──► Domain
```

The **Domain** project has no dependencies. **Application** defines the repository and service contracts, so business rules never reference EF Core or ASP.NET Core. **Infrastructure** implements those contracts. The **API** project only wires everything together.

## API endpoints

| Method | Route | Access | Description |
| --- | --- | --- | --- |
| `POST` | `/api/auth/register` | Public | Create an account and receive a JWT |
| `POST` | `/api/auth/login` | Public | Authenticate and receive a JWT |
| `GET` | `/api/cars` | Public | List cars; optional `search`, `availableFrom`, `availableTo` |
| `GET` | `/api/cars/{id}` | Public | Get a single car |
| `POST` | `/api/cars` | Admin | Add a car |
| `PUT` | `/api/cars/{id}` | Admin | Update a car |
| `DELETE` | `/api/cars/{id}` | Admin | Delete a car without booking history |
| `GET` | `/api/bookings/availability` | Public | Check availability for `carId`, `startDate`, `endDate` |
| `POST` | `/api/bookings` | Customer | Create a booking |
| `GET` | `/api/bookings/mine` | Customer | List the current user's bookings |
| `POST` | `/api/bookings/{id}/cancel` | Customer/Admin | Cancel a booking |
| `GET` | `/api/bookings` | Admin | List all bookings |
| `PATCH` | `/api/bookings/{id}/status` | Admin | Change a booking's status |

Full request/response examples live in [`back-end/Docs/Api.md`](back-end/Docs/Api.md).

## Getting started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Node.js 20+ and npm
- SQL Server (LocalDB, Express or Developer edition)

### 1. Configure the database

The default connection string in `back-end/back-end.Api/appsettings.json` uses the local default SQL Server instance:

```json
"Server=localhost;Database=RunnerCarRentals;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
```

Change it if you use a different instance (for example `Server=localhost\\SQLEXPRESS;...`).

### 2. Run the API

```bash
cd back-end
dotnet restore
dotnet run --project back-end.Api
```

On startup the API applies EF Core migrations and seeds:

- an admin account (development defaults: `admin@runnercars.local` / `Admin123!` — override them with `SeedAdmin:Email` and `SeedAdmin:Password`)
- eight sample vehicles, if the fleet is empty

Swagger UI: <http://localhost:5031/swagger> (the `http` launch profile is the default; in Development the API does not force HTTPS so the Angular dev proxy works without a trusted certificate).

> For production, store `JwtSettings:Secret` and the seed admin password in user secrets or environment variables. Never reuse the development values.

### 3. Run the frontend

```bash
cd front-end
npm install
npm start
```

The Angular dev server runs on <http://localhost:4200> and proxies `/api` to `http://localhost:5031` (see `front-end/proxy.conf.json`).

### 4. Try the flow

1. Browse **Vehicles** and filter by pick-up/drop-off dates.
2. Register a customer account and request a booking.
3. Sign in as the seeded admin and confirm the booking in **Admin → Bookings**.
4. Sign back in as the customer to see the confirmed status.

## Project structure

```
runnerCarRentals-fullstack/
├── back-end/
│   ├── back-end.Api/             # Controllers, middleware, Program.cs
│   ├── back-end.Application/     # Services, interfaces, business rules
│   ├── back-end.Contracts/       # DTOs
│   ├── back-end.Domain/          # Entities and enums
│   ├── back-end.Infrastructure/  # EF Core, repositories, JWT, hashing
│   └── Docs/                     # API and domain documentation
├── front-end/
│   └── src/app/
│       ├── core/                 # Models, services, guards, interceptor
│       ├── components/           # Navbar
│       └── pages/                # Home, catalog, detail, bookings, admin, auth
└── README.md
```

## Engineering decisions worth highlighting

- **Availability is calculated, not stored.** Instead of a stale `isBooked` flag, the overlap query `StartDate < requestedEnd && requestedStart < EndDate` excludes cancelled bookings and is used both for search and for booking validation.
- **Business rules are unit-testable without a database.** `BookingService` depends on repository interfaces and `IDateTimeProvider`, so date and pricing rules can be verified with fakes.
- **Auth concerns are split.** `Application` defines `IPasswordHasher` and `IJwtTokenGenerator`; `Infrastructure` implements them, keeping the core free of cryptography and JWT libraries.
- **Errors are first-class.** Typed application exceptions (`NotFound`, `Conflict`, `BadRequest`, `Unauthorized`) are mapped to RFC 7807 ProblemDetails by a single middleware.
- **The client is typed and guarded.** Shared DTO models, a functional interceptor for bearer tokens and route guards keep auth handling out of individual components.

## Roadmap

Planned next steps — the goal is to grow this same stack (Angular + ASP.NET Core + SQL Server) into an AI-augmented application:

- [ ] **AI rental assistant** in the Angular client: natural-language requests (“I need a 7-seater for 5 days under R6 000”) translated by an LLM into structured search filters and tool calls against the existing API.
- [ ] **RAG over rental policies** (cancellation, insurance, mileage) using SQL Server as the system of record and a vector store for retrieval.
- [ ] **Demand forecasting and dynamic pricing** service that recommends daily rates from historical bookings.
- [ ] **Automated booking triage**: LLM-assisted risk/exception summaries for admins before confirming a booking.
- [ ] Payments, email notifications, integration tests and a CI/CD pipeline.

## Author

Built by [Tlou Manaka](https://github.com/TlouManaka). Feedback and suggestions are welcome via issues.
