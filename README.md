# ProjectZero

A .NET 10 Blazor (interactive server) web application built on a **Clean Architecture** / **Domain-Driven Design (DDD)** foundation. It demonstrates a pragmatic CQRS + MediatR setup layered into Domain, Application, Infrastructure, and Presentation projects.

## Table of Contents

- [Tech Stack](#tech-stack)
- [Solution Structure](#solution-structure)
- [Architecture](#architecture)
- [Features](#features)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Running Tests](#running-tests)

## Tech Stack

| Area        | Technology                                              |
|-------------|---------------------------------------------------------|
| Framework   | .NET 10, ASP.NET Core Blazor (Interactive Server)       |
| Language    | C#                                                      |
| CQRS        | MediatR                                                 |
| Persistence | EF Core (SQL Server LocalDB, in-memory for dev)         |
| UI          | Bootstrap 5 + Bootstrap Icons                           |
| Testing     | xUnit, Moq, FluentAssertions (where applicable)         |

## Solution Structure

```
ProjectZero.sln
├── Core.Domain            # Entities, aggregate roots, domain events, interfaces
├── Core.Application       # CQRS commands/queries, handlers, DTOs, MediatR setup
├── Core.Infrastructure    # EF Core DbContext, repositories, unit of work, migrations
├── ProjectZero            # Blazor web app (components, pages, layout)
├── Shared                 # Shared assets/utilities
└── Tests                  # Unit/integration test projects
```

## Architecture

**Clean Architecture** with a strict dependency rule: each layer depends only on the layers beneath it.

- **Core.Domain** — the heart of the system. Contains domain entities (e.g. `Product`, `User`), aggregate roots, domain events, and repository/unit-of-work interfaces. No external dependencies.
- **Core.Application** — application orchestration. Implements MediatR commands and queries (write/read operations), DTOs, and validation. Depends only on `Core.Domain`.
- **Core.Infrastructure** — persistence and infrastructure concerns. EF Core `ApplicationDbContext`, concrete repositories, the unit of work, and EF migrations. Depends on `Core.Domain` and `Core.Application`.
- **ProjectZero** — the Blazor UI. Wires everything together through dependency injection and MediatR's `ISender`.

### DDD & CQRS highlights

- **Aggregate roots** encapsulate business rules, e.g. `Product.Create(...)` and `Product.Update(...)` validate invariants internally and raise **domain events** (`ProductCreatedEvent`, `ProductUpdatedEvent`, etc.).
- **Repository pattern** (`IProductRepository`, `IUserRepository`) plus a **Unit of Work** (`IUnitOfWork`) to coordinate persistence in a single transaction.
- **CQRS** separates reads (`*Query`) from writes (`*Command`), dispatched through **MediatR**. Handlers are resolved by convention at runtime, so the UI only ever talks to `ISender`.

## Features

- Blazor **Interactive Server** components with Bootstrap 5 styling.
- Product catalog with a responsive, resize-aware grid (`ProductRow`) and cards (`ProductCard`).
- Create, edit (via a Bootstrap modal), delete, and list products.
- Child-to-parent component communication through typed `EventCallback<T>` parameters.
- EF Core with a SQL Server LocalDB database and an initial migration that is applied automatically on startup.

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server LocalDB (included with Visual Studio) for the default configuration

### Run

```bash
dotnet restore
dotnet build ProjectZero.sln
dotnet run --project ProjectZero/ProjectZero/ProjectZero.csproj
```

The app listens on the Kestrel URLs shown in the terminal output (typically `https://localhost:<port>`). On startup it applies pending EF Core migrations automatically (`Program.cs`), so the schema is created for you.

Navigate to `/product-creator` (under the **Playground** menu) to try the product create/edit/delete flows.

## Configuration

Connection settings live in `ProjectZero/ProjectZero/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Server=(localdb)\\MSSQLLocalDB;Database=ProjectZero;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

`Core.Infrastructure` also exposes an overload of `AddInfrastructureServices` that uses an **in-memory** EF Core provider when no connection string is supplied — handy for demos and tests.

## Running Tests

```bash
dotnet test ProjectZero.sln
```

Test projects are located under `Tests/`, covering the domain, application (handlers), and infrastructure layers.

---

**License:** This is a demonstration project. See the source files for details.