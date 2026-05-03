# ArkServerManager.Admin

Local operator Blazor admin UI (Interactive Server) for ArkServerManager.

## Development run sequence

1. Start `ArkServerManager.Worker` first (default `http://127.0.0.1:5080`).
2. Configure Admin UI Worker settings (do not commit real keys):
   - `Worker:BaseUrl` (default `http://127.0.0.1:5080/`)
   - `Worker:ApiKey` (must match Worker `ApiAuth:ApiKey`)
3. Start `ArkServerManager.Admin` (default `http://127.0.0.1:5081`).

### Set API key for development

Prefer User Secrets or environment variables over checked-in config.

Example with user-secrets:

```bash
dotnet user-secrets --project src/ArkServerManager.Admin set "Worker:ApiKey" "your-dev-api-key"
```

Example with environment variable (double underscore maps to nested config):

```bash
export ARKMGR_ADMIN_Worker__ApiKey="your-dev-api-key"
```

The UI keeps the API key server-side and calls Worker `/api/v1/...` using the `X-Api-Key` header.
