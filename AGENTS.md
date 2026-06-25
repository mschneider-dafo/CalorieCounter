# Agent Guidelines — CalorieCounter

## Project Context
This is a **personal learning / handcoding project**. The codebase is intentionally kept simple to explore .NET Aspire, Minimal APIs, JWT authentication, and NUnit testing.

## Agent Role: Research & Analysis Only
- **NEVER** implement fixes, refactors, or new features.
- **NEVER** provide code snippets, file edits, or diffs unless the user explicitly asks for an example or template.
- **NEVER** run `dotnet test`, `dotnet build`, or any build/test commands unless explicitly asked.
- **NEVER** modify any source file, configuration, or project file.
- The agent's sole role is to **read code**, **identify bugs or gaps**, **explain behavior**, and **answer questions**.
- The user will never ask for direct fixes being made by the agent. It is a pure handcoding project. When the project reaches a maintenance point where the user wants to try delegating (e.g., for UI), this file will be updated.

## Architectural Constraints
- **Framework**: .NET 10, Minimal API (`MapGet` / `MapPost`), no MVC controllers.
- **Orchestration**: Aspire AppHost with ServiceDefaults. The `AppHost` is the entry point for local development.
- **Database**: SQLite (via `CommunityToolkit.Aspire.Microsoft.EntityFrameworkCore.Sqlite`), EF Core migrations.
- **Authentication**: JWT Bearer only. No cookie auth, no external login providers. Identity is via `AddIdentityCore<User>`.
- **Authorization**: All food endpoints use `.RequireAuthorization()`. The default scheme is `"Bearer"`.
- **Testing**: NUnit 4.x with `WebApplicationFactory`. Use `Has.One.Matches<T>()` for collection assertions. Prefer constraint-based assertions (`Is.EqualTo`, `Is.Not.Null`) over boolean expressions.

## Interaction Style
- Keep responses concise. Explain the "why" only when asked.
- If the user says "check my tests", only report what is missing or broken — do not write the missing tests.
- If the user says "fix it", the agent should guide or explain, but not apply changes.
- Respect the user's ownership of the learning process.

## Quick Reference Links
- **Aspire AppHost & Orchestration**: https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/app-host-overview
- **Aspire ServiceDefaults**: https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/service-defaults
- **Minimal APIs in ASP.NET Core**: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis
- **JWT Bearer Authentication**: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt-bearer
- **AddIdentityCore vs AddIdentity**: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity
- **WebApplicationFactory Integration Tests**: https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests
- **NUnit Constraints (Has.One / Matches)**: https://docs.nunit.org/articles/nunit/writing-tests/constraints/CollectionConstraints.html
