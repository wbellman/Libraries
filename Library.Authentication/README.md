# Library.Authentication

`Library.Authentication` is a .NET class library that provides authentication-focused building blocks: user and credential persistence models, JWT generation, and password token creation/verification utilities.

## Problem it explores

The project explores how to keep authentication concerns in a reusable library by combining:
- Entity Framework Core mappings for users and credentials
- strongly-typed request/result models
- JWT issuance with configurable settings
- credential token generation and password verification primitives

## Tech stack

- C# / .NET 8 (`net8.0`)
- Entity Framework Core (`Microsoft.EntityFrameworkCore`, `Relational`)
- Microsoft Options/configuration abstractions
- `System.IdentityModel.Tokens.Jwt`
- .NET cryptography APIs (`Rfc2898DeriveBytes`, `Aes`, `RandomNumberGenerator`)

## Build

From the repository root:

```bash
dotnet restore Libraries.sln
dotnet build Library.Authentication/Library.Authentication.csproj --no-restore
```

## Core concepts

See [docs/CORE_CONCEPTS.md](docs/CORE_CONCEPTS.md) for detailed techniques demonstrated by this project.
