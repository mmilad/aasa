# ArkServerManager UI (Next.js POC)

This is a **POC** operator admin UI for `ArkServerManager.Worker`.

It has **no human login system**. The UI never calls the Worker API directly from the browser; instead it uses Next.js **server routes** that inject the Worker header `X-Api-Key` server-side.

## Worker requirements

The Worker API must be running and reachable at:

- `WORKER_BASE_URL` (default): `http://127.0.0.1:5080/`
- `ApiAuth:ApiKey` must be set in the Worker config (Development example: `CHANGE_ME_USE_ENV_OR_LOCAL_FILE`)

## Configure Worker API key

Create `apps/web/.env.local`:

```bash
WORKER_BASE_URL=http://127.0.0.1:5080/
WORKER_API_KEY=CHANGE_ME_USE_ENV_OR_LOCAL_FILE
```

## Run

### 1) Start the Worker API

In the repo:

```powershell
cd apps\ArkServerManager.Worker
dotnet run
```

Keep it running.

### 2) Start the Next.js UI

From the **repository root**:

```powershell
pnpm dev
```

Open `http://127.0.0.1:3000`.

## Manual end-to-end checklist (MVP)

1. UI: confirm server list loads (it may be empty).
2. UI: `Create server` with name/map/session and optional ports.
3. UI (server detail): click **Install/Update** (optional for your local setup).
4. UI: click **Start**.
5. UI: observe the **Job panel** polling until status becomes `Done` or `Failed`.
6. UI: click **Stop** and verify the Worker server state updates.

## Verification (developer)

- Backend unit tests:

From the **repository root**:

```powershell
dotnet test ArkServerManager.sln
```

- UI build:

From the **repository root**:

```powershell
pnpm build
```
