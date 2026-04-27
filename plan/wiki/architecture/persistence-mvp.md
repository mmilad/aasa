# Persistence model — MVP tables and states

## Purpose

Define **MVP** relational schema and state machines. Post-MVP entities (`Cluster`, full `Mod` registry) are **out of schema** until ADR expands scope — see [mvp-scope.md](../requirements/mvp-scope.md).

## Tables (MVP)

### `servers`

| Column | Type | Notes |
|--------|------|--------|
| `id` | UUID PK | lowercase string in API |
| `name` | text | display |
| `created_at` | datetime | UTC |
| `updated_at` | datetime | UTC |
| `state` | text | enum `ServerState` |
| `install_root` | text | absolute path to `...\servers\{id}\binaries` (SteamCMD `force_install_dir`) |
| `game_port` | int | default `7777` |
| `query_port` | int | default `27015` |
| `rcon_port` | int | e.g. `27020` |
| `rcon_password` | text | **not logged**; ACL on DB file |
| `map_name` | text | e.g. `TheIsland_WP` — must match ASA |
| `session_name` | text | |
| `pid` | int nullable | OS pid when running |
| `last_job_id` | UUID nullable | optional convenience |

### `jobs`

| Column | Type | Notes |
|--------|------|--------|
| `id` | UUID PK | |
| `server_id` | UUID FK | |
| `type` | text | `InstallOrUpdate` \| `Start` \| `Stop` \| `Restart` \| `Backup` |
| `status` | text | `JobStatus` |
| `created_at` | datetime | |
| `started_at` | datetime nullable | |
| `finished_at` | datetime nullable | |
| `exit_code` | int nullable | SteamCMD / process |
| `log_blob` | text nullable | tail or full stdout capture (size cap TBD impl) |

## Enums

### `ServerState`

`Stopped` → `Starting` → `Running` → `Stopping` → `Stopped`  
Also: `Updating`, `Crashed`, `Error` (terminal-ish; recovery path = operator action or `Stop`).

Allowed transitions (normative):

- `Stopped` + action start → `Starting`
- `Starting` + process alive → `Running`
- `Starting` + failure → `Error`
- `Running` + stop → `Stopping` → `Stopped`
- `Running` + crash detect → `Crashed`
- Any non-terminal + `InstallOrUpdate` job start → `Updating` (or reject with 409 — see [jobs-mvp.md](../requirements/jobs-mvp.md))

### `JobStatus`

`Pending` → `Running` → `Done` | `Failed`

## Out of MVP (explicit)

- `clusters`, `mods` tables — **not** created in v0.1.
- Encrypted-at-rest columns — optional post-MVP (ADR-0003).

## Agent acceptance checklist

- [ ] EF entities match columns above (names may be PascalCase in C#).
- [ ] Migrations create only these tables + FK index on `jobs.server_id`.
- [ ] Repository layer is the only place that touches `DbContext` from domain logic.

## Sources

- [persistence-providers.md](persistence-providers.md)
- [product/00-foundations.md](../../product/00-foundations.md)
