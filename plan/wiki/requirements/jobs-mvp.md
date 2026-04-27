# Jobs and concurrency — MVP

## Purpose

Prevent SteamCMD / process races that cause **file lock** failures and corrupted installs (see [asa-known-issues.md](../risks/asa-known-issues.md)).

## Job types (MVP)

| `type` value | Enqueues when |
|----------------|---------------|
| `InstallOrUpdate` | `POST .../jobs/install` |
| `Start` | `POST .../actions/start` (may run synchronously; still record row) |
| `Stop` | `POST .../actions/stop` |
| `Restart` | `POST .../actions/restart` |
| `Backup` | `POST .../jobs/backup` |

## Concurrency rules (normative)

1. **At most one** `InstallOrUpdate` job **Running** **globally** across all servers (singleton SteamCMD worker).
2. `InstallOrUpdate` **must not** enter `Running` if target server’s game process is **not** `Stopped` → API returns **`409 CONFLICT`** with code `SERVER_NOT_STOPPED`.
3. **At most one** long-running job per `serverId` in `Running` (no overlapping install + start).
4. `Start` while `InstallOrUpdate` pending/running for same server → **`409`** `INSTALL_IN_PROGRESS`.
5. `Backup` job **requires** `ServerState == Stopped` (avoid zipping locked files); same global SteamCMD singleton does not apply—**Backup** may run parallel to idle SteamCMD but not during `InstallOrUpdate` **Running** (serialize with file lock or `409`).

## Port strategy (MVP)

- **Operator-assigned** `game_port`, `query_port`, `rcon_port` stored per server (defaults 7777 / 27015 / 27020).
- On `POST /api/v1/servers`, validate **TCP not in use** on bind addresses the game will use (localhost check minimum); return **`400 VALIDATION_FAILED`** if taken.
- Document in runbook: external firewall must open the same ports for internet listing.

## Agent acceptance checklist

- [ ] Second global SteamCMD job gets `409` or queues as `Pending` behind first — **pick one** in impl and document (queue preferred).
- [ ] Install rejected when `ServerState != Stopped`.
- [ ] Port conflict returns `400` with field hint in `error.detail`.

## Sources

- [api-v1-mvp.md](api-v1-mvp.md)
- [asa-known-issues.md](../risks/asa-known-issues.md)
