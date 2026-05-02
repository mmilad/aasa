# arkservermanager

Planning and future implementation for an **ARK: Survival Ascended** dedicated server manager.

**Start here:** [plan/README.md](plan/README.md)

## Local admin UI (Phase C5) quickstart

The repository now includes a local Blazor admin project at `src/ArkServerManager.Admin`.

Default loopback ports:

- Worker API: `http://127.0.0.1:5080`
- Admin UI: `http://127.0.0.1:5081`

Run order for development:

1. Start Worker (`src/ArkServerManager.Worker`).
2. Configure Admin Worker API key (must match Worker `ApiAuth:ApiKey`) via user-secrets or environment variables.
3. Start Admin (`src/ArkServerManager.Admin`) and open `http://127.0.0.1:5081`.

Example dev secret setup:

```bash
dotnet user-secrets --project src/ArkServerManager.Admin set "Worker:ApiKey" "your-dev-api-key"
```

```bash
export ARKMGR_ADMIN_Worker__ApiKey="your-dev-api-key"
```
