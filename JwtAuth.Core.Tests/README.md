# JwtAuth.Core.Tests

This test project verifies the behavior and security guarantees of `JwtAuth.Core`.

## What Is Covered

- **Token Generation Basics**
  - The library returns a non-empty JWT string.
  - Generated tokens include expected issuer, audience, and core claims (`sub`, `unique_name`, `jti`).

- **Configuration Guardrails**
  - Invalid `JwtSettings` values fail fast with clear exceptions.
  - Missing `SecretKey` in DI configuration is rejected early.

- **Expiration Behavior**
  - Tokens generated with short expiration are validated against an expected time window.

- **Signature Integrity**
  - A valid token passes signature validation with the configured secret.
  - Validation with a wrong secret fails.
  - Payload tampering (modifying claims without re-signing) fails signature validation.

## Why This Matters

These tests demonstrate practical JWT security fundamentals:

- Authenticity: only tokens signed with the correct key are accepted.
- Integrity: modified token payloads are detected and rejected.
- Safety by default: invalid configuration is caught before runtime surprises.

## Run Tests

From repository root:

```bash
dotnet test JwtAuthShowcase.sln
```
