# Library.Authentication Core Concepts

This document describes notable techniques observable in `Library.Authentication` source code.

## 1) EF Core context with assembly-scanned configuration

### What is implemented
- `AuthenticationContext` inherits from `DbContext` and exposes `DbSet<User>` and `DbSet<Credential>`.
- `OnModelCreating` uses `ApplyConfigurationsFromAssembly(GetType().Assembly)` to register entity mappings from configuration classes.
- Entity mappings are split into `Configurations/Users.cs` and `Configurations/Credentials.cs`.

### Why
> TODO(wbellman): why — explain the design choice to keep mapping logic in separate `IEntityTypeConfiguration<T>` classes and register them via assembly scanning.

## 2) Owned value object mapping for credential token data

### What is implemented
- `Credential` contains a `CredentialToken` model with `Salt`, `Vector`, and `EncryptedToken`.
- The `Credentials` configuration maps `Token` using `OwnsOne`, constraining field lengths and requiredness at the database level.
- Credential rows also enforce key generation, required timestamps, validity defaults, and an index on `UserId`.

### Why
> TODO(wbellman): why — explain why token components are modeled as an owned type and persisted with these specific constraints.

## 3) JWT generation through options-driven settings

### What is implemented
- `TokenService` receives `IOptions<JwtSettings>` and reads `SecretKey`, `Issuer`, `Audience`, and `ExpiryMinutes`.
- `GenerateJwt` creates standard identity claims (`sub`, `unique_name`, `jti`, `nameidentifier`, `email`) and signs with HMAC SHA-256 symmetric keys.
- Token lifetime is computed from UTC time and configured expiration minutes.

### Why
> TODO(wbellman): why — describe the rationale for this specific claim set and configuration model for token issuance.

## 4) Password-derived credential token workflow

### What is implemented
- `Create` generates a random salt.
- `Create` derives a 256-bit key from password + salt using PBKDF2 (`Rfc2898DeriveBytes`, SHA-256, 10,000 iterations).
- `Create` then (as implemented) encrypts the derived key bytes using that same derived key as the AES-CBC/PKCS7 key, and stores Base64 salt/IV/ciphertext in `CredentialToken`.
- `CheckPassword` rebuilds the derived key from the incoming password and stored salt, re-encrypts with the stored IV, and compares ciphertext bytes to validate.
- Helper methods isolate cryptographic primitives (`GenerateSalt`, `DeriveKeyFromPassword`, encryption helpers, byte comparison).

### Security observations from the current implementation
- The configured PBKDF2 iteration count is `10000`; this is below OWASP-cited guidance for PBKDF2-HMAC-SHA256 (for example, 600,000 as of 2023) and should be treated as a security concern requiring revision.
- AES-CBC is used without an authentication step in this flow; this should be treated as a security concern requiring review of authenticated alternatives.

### Why
> TODO(wbellman): why — explain the intended security trade-offs and why this token-verification approach was chosen over alternative password storage/verification patterns.
> TODO(wbellman): why — explain why PBKDF2 iteration count `10000` was selected and whether it should be revised against current guidance.
> TODO(wbellman): why — explain why AES-CBC was selected here and whether authenticated encryption alternatives (for example AES-GCM/AEAD) were evaluated.

## 5) Record-based request and response boundary models

### What is implemented
- `Models/Requests.cs` defines nested request records for user creation and login payloads.
- `ValidatedUser` and `CredentialToken` are immutable record models used for data transfer.
- `JwtSettings` is a mutable options-binding model for configuration providers.

### Why
> TODO(wbellman): why — explain where immutable records vs mutable options models fit in the project’s boundary design.
