# MVP scope and non-goals (gate G1)

This page is **falsifiable**: if a feature is not listed in “In scope for MVP v0.1”, it is out of scope unless an ADR explicitly expands MVP.

## Product definition (MVP v0.1)

**In scope for MVP v0.1** — the smallest manager that is still useful on **Windows Server**:

1. **Single-machine** deployment: manager process and managed ASA dedicated server(s) on the **same Windows Server host** (no multi-node orchestration).
2. **SteamCMD install/update** of ASA dedicated server build to a per-server directory under the [data layout contract](../architecture/data-layout-windows.md).
3. **Process lifecycle**: start / stop / restart dedicated server process; capture stdout/stderr; persist PID and state via **EF Core** ([persistence-providers.md](../architecture/persistence-providers.md)) default **SQLite**, optional **PostgreSQL**.
4. **One REST API** (versioned prefix `/api/v1/...`) — normative route table: [api-v1-mvp.md](api-v1-mvp.md). Polling for logs; **no WebSocket in MVP**.
5. **RCON**: configure password/port; send shutdown attempt before kill; minimal “list players” if protocol permits without heroic parsing (otherwise document “phase 1.1”).
6. **INI safety**: read/write under `...\ShooterGame\Saved\Config\WindowsServer\` with validation and **backup-before-write** — [product/06-config-ini.md](../../product/06-config-ini.md), [raw/asa-install-layout-windows.md](../../raw/asa-install-layout-windows.md).
7. **Local auth for API**: shared secret / API key model per [ADR-0003](../decisions/ADR-0003-secrets-and-exposure.md).
8. **Local admin UI (Blazor)**: first-party **operator panel** in **C#** using **Blazor Web App (Interactive Server)** against the REST API — [ADR-0004](../decisions/ADR-0004-local-admin-ui-blazor.md). Loopback-first; same secret handling discipline as ADR-0003.

## Explicit non-goals (MVP v0.1)

- **No** **public Internet** control dashboard (anonymous or unauthenticated browser exposure to the world). **Local / same-host** Blazor admin is **in scope** per [ADR-0004](../decisions/ADR-0004-local-admin-ui-blazor.md).
- **No** normative first-party **TypeScript SPA** for the operator UI — .NET-only UI per ADR-0004.
- **No** external plugin host protocol, gRPC plugin registry, or third-party extension marketplace — deferred per [ADR-0002](../decisions/ADR-0002-plugin-integration.md).
- **No** Discord bot, Prometheus, Grafana, Docker image — tracked in product docs as later phases.
- **No** CurseForge mod browser/install automation beyond “manual path registration” unless a later milestone ADR adds it.
- **No** Linux-hosted manager or Linux-native ASA assumptions — Windows only per [ADR-0001](../decisions/ADR-0001-host-and-deployment.md).
- **No** clustered cross-server orchestration beyond documenting future hooks (no cluster sync implementation in v0.1).

## MVP success criteria (binary)

MVP v0.1 is **done** when all are true on a clean Windows Server VM:

1. Operator can install manager as a **Windows Service** (documented steps) and reach API on the configured loopback interface.
2. Operator can register **one** server, run SteamCMD update to populate binaries, start server, see logs via API, stop server gracefully (RCON attempt + fallback).
3. Operator can change **one** documented INI key via API and see it reflected on disk with a `.bak` file retained.
4. No secrets appear in application logs at default log level (see ADR-0003).
5. Operator can open the **local Blazor admin** on loopback (per ADR-0004), authenticate with the configured API key pattern, and **list servers** plus **enqueue at least one job** (install, start, or backup) with visible terminal/job outcome in the UI or linked detail view.

## Sources

- [ADR-0001](../decisions/ADR-0001-host-and-deployment.md)
- [ADR-0003](../decisions/ADR-0003-secrets-and-exposure.md)
- [ADR-0004](../decisions/ADR-0004-local-admin-ui-blazor.md)
- [product/08-api.md](../../product/08-api.md)
- [api-v1-mvp.md](api-v1-mvp.md)
- [production-readiness.md](production-readiness.md)
