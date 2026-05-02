# Technology stack (implementation language)

**Decision:** Build the manager in **C# on .NET**, hosted as a **Windows Service** using **.NET Worker Service** + **ASP.NET Core** (Kestrel in-process).

## Runtime

- **Target framework**: **.NET 8** (LTS) — upgrade path to newer LTS documented at ship time, not a planning blocker.
- **Process model**: one OS service hosting Kestrel + background hosted services (process supervisor, job queue).

## Libraries (non-binding list for planning — versions pinned at implementation)

- **Configuration**: `Microsoft.Extensions.Configuration` (JSON + env vars).
- **Logging**: `Microsoft.Extensions.Logging` with structured JSON file sink TBD at implementation (Serilog optional).
- **HTTP**: ASP.NET Core Minimal APIs for REST v1.
- **Operator UI**: **Blazor Web App (Interactive Server)** in a **separate** ASP.NET Core project calling the Worker REST API — [ADR-0004](../decisions/ADR-0004-local-admin-ui-blazor.md). Normative **not** a first-party TypeScript SPA.
- **Database**: **EF Core** with **SQLite default** and **PostgreSQL optional** via configuration — see [persistence-providers.md](persistence-providers.md) and [persistence-mvp.md](persistence-mvp.md).
- **SteamCMD interaction**: `System.Diagnostics.Process` with async stdout/stderr reads.

## Out of scope for stack choice

- Game server is **not** .NET; it remains the ASA Windows dedicated binary launched as subprocess.

## Sources

- [ADR-0001](../decisions/ADR-0001-host-and-deployment.md)
- [ADR-0004](../decisions/ADR-0004-local-admin-ui-blazor.md)
- [.NET Worker Service overview](https://learn.microsoft.com/en-us/dotnet/core/extensions/windows-service)
