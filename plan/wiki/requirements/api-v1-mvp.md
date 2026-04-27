# REST API v0.1 (MVP) — normative contract

## Purpose

Define every **MVP** HTTP surface so implementers do not invent routes or shapes during Phase C3. **WebSocket is out of MVP** — use polling for logs/status.

## Auth

- **Header**: `X-Api-Key: <shared secret>` on every route **except** `GET /health` (unauthenticated for local ops probes).
- **401**: missing or wrong key → body uses error envelope below.
- Source: [ADR-0003](../decisions/ADR-0003-secrets-and-exposure.md).

## ID formats

- `serverId`, `jobId`: **UUID** string, lowercase, no braces (regex `^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$`).

## Error envelope (MVP)

All non-2xx JSON responses (when `Accept` includes `application/json` or always for `/api/v1`):

```json
{
  "error": {
    "code": "SNAKE_CASE_STABLE",
    "message": "Human readable",
    "detail": {}
  }
}
```

- **`detail`**: optional object (field errors, hints). Never put secrets in `message`/`detail`.
- **Common codes**: `UNAUTHORIZED`, `NOT_FOUND`, `CONFLICT`, `VALIDATION_FAILED`, `JOB_FAILED`, `PRECONDITION_FAILED` (e.g. install while server running).

## Route table (MVP)

| Method | Path | Auth | Purpose | Success |
|--------|------|------|---------|---------|
| `GET` | `/health` | no | Liveness + build id | `200` JSON `{ "status":"ok", "version":"semver" }` |
| `GET` | `/api/v1/servers` | yes | List servers | `200` `{ "items": [ ServerSummary ] }` |
| `POST` | `/api/v1/servers` | yes | Create server + disk tree | `201` `Server` |
| `GET` | `/api/v1/servers/{serverId}` | yes | Get server | `200` `Server` |
| `DELETE` | `/api/v1/servers/{serverId}` | yes | Delete registry + optional dirs policy | `204` or `200` with body per impl note in persistence |
| `POST` | `/api/v1/servers/{serverId}/jobs/install` | yes | Enqueue SteamCMD `2430930 validate` | `202` `{ "jobId" }` |
| `POST` | `/api/v1/servers/{serverId}/jobs/backup` | yes | Create zip of config + SavedArks | `202` `{ "jobId" }` |
| `POST` | `/api/v1/servers/{serverId}/actions/start` | yes | Start process | `202` or `200` + job id if async |
| `POST` | `/api/v1/servers/{serverId}/actions/stop` | yes | Graceful stop | `202` or `200` |
| `POST` | `/api/v1/servers/{serverId}/actions/restart` | yes | stop → start | `202` or `200` |
| `GET` | `/api/v1/servers/{serverId}/status` | yes | Runtime snapshot | `200` `{ "state","pid","ports",... }` |
| `GET` | `/api/v1/servers/{serverId}/logs` | yes | Tail text | `200` `{ "lines": ["..."], "truncated": bool }` query `?tail=500` default 200 |
| `GET` | `/api/v1/jobs/{jobId}` | yes | Job status + log excerpt | `200` `Job` |
| `GET` | `/api/v1/servers/{serverId}/ini/GameUserSettings` | yes | Read INI as structured JSON | `200` |
| `GET` | `/api/v1/servers/{serverId}/ini/Game.ini` | yes | Read-only | `200` |
| `PATCH` | `/api/v1/servers/{serverId}/ini/GameUserSettings` | yes | Mutate **allowlist** key only | `200` see [06-config-ini](../../product/06-config-ini.md) |

**`Server` / `ServerSummary` / `Job`**: JSON shapes defined at implementation time but **fields** must cover [product/00-foundations.md](../../product/00-foundations.md) server model + job id/status.

## HTTP status rules (MVP)

| Situation | Status |
|-----------|--------|
| Success list/get | `200` |
| Created server | `201` |
| Deleted | `204` |
| Async accepted | `202` + `Location: /api/v1/jobs/{jobId}` optional |
| Bad JSON / allowlist violation | `400` + `VALIDATION_FAILED` |
| Missing auth | `401` |
| Unknown id | `404` |
| Install while server running / SteamCMD lock | `409` + `CONFLICT` |
| Server in wrong state | `409` or `422` (pick one in code and document) |

## Agent acceptance checklist (copy-paste)

- [ ] Every route in the table exists and matches method + path prefix `/api/v1` (except `/health`).
- [ ] Wrong/missing `X-Api-Key` → `401` with error envelope on all `/api/v1/*` routes.
- [ ] `/health` returns `200` without key.
- [ ] Unknown `serverId` / `jobId` → `404` with envelope.
- [ ] `POST .../jobs/install` when game process running → `409` (see [jobs-mvp.md](jobs-mvp.md)).
- [ ] `POST .../jobs/backup` when server not stopped → `409` (see [jobs-mvp.md](jobs-mvp.md)).
- [ ] No response body at `info` log level includes API key, RCON password, or Steam password.
- [ ] OpenAPI 3.0 document checked in under `openapi/` or embedded doc — **implementation choice**, but file must exist before closing C3.1.

## Sources

- [mvp-scope.md](mvp-scope.md)
- [ADR-0003](../decisions/ADR-0003-secrets-and-exposure.md)
- [jobs-mvp.md](jobs-mvp.md)
