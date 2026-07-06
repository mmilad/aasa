# arkservermanager

Planning and implementation for an **ARK: Survival Ascended** dedicated server manager (Windows-first MVP).

**Start here:** [plan/README.md](plan/README.md)

## Repository layout

| Path | Purpose |
|------|---------|
| [plan/](plan/) | Normative product and architecture docs |
| [openapi/](openapi/) | OpenAPI artifact for Worker `/api/v1` |
| [apps/ArkServerManager.Worker/](apps/ArkServerManager.Worker/) | Worker service (REST API, jobs, SteamCMD/process integration) |
| [apps/web/](apps/web/) | Next.js operator UI (loopback-first; proxies to Worker server-side) |
| [tests/ArkServerManager.Worker.Tests/](tests/ArkServerManager.Worker.Tests/) | .NET unit / integration tests |

## .NET (Worker + tests)

From the repository root:

```powershell
dotnet build ArkServerManager.sln
dotnet test ArkServerManager.sln
```

Default Worker API URL in dev is typically `http://127.0.0.1:5080` (see Worker `appsettings.Development.json` and [plan/wiki/architecture/asa-process-runbook.md](plan/wiki/architecture/asa-process-runbook.md)).

## Next.js admin UI

Requires **Node 20+** and [pnpm](https://pnpm.io/).

```powershell
pnpm install
pnpm dev
```

The web app reads **`WORKER_BASE_URL`** and **`WORKER_API_KEY`** at runtime (server-side only; do not prefix the key with `NEXT_PUBLIC_`). Example:

```powershell
$env:WORKER_BASE_URL = "http://127.0.0.1:5080/"
$env:WORKER_API_KEY = "your-dev-api-key"
pnpm dev
```

Copy [apps/web/.env.local.example](apps/web/.env.local.example) to `apps/web/.env.local` if you prefer file-based env for Next.

Production build (from repo root):

```powershell
pnpm build
```

## Agent / contributor bootstrap

See [AGENTS.md](AGENTS.md) for spec-first workflow and coding order.
