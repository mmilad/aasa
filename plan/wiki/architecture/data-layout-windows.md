# Data directory contract — Windows (gate G4)

Frozen contract for **on-disk layout** the manager owns. Implementation must match these paths and names unless an ADR revises this document.

## Root

- **Configurable** `ManagerDataRoot` (application setting + env override `ARKMGR_DATA_ROOT`).
- **Default** (when unset): `%ProgramData%\ArkServerManager\`
- **Invariant**: all mutable data lives under `ManagerDataRoot` except OS temp if explicitly used for staging (must be documented).

## Per-server tree

Path pattern (logical; physical Windows path):

`{ManagerDataRoot}\servers\{serverId}\`

| Subdirectory | Purpose |
|--------------|---------|
| `binaries\` | SteamCMD install target for ASA dedicated binaries for this server |
| `configs\` | Manager-owned JSON sidecars / optional mirrors; **canonical INI paths for MVP** live under `binaries\ShooterGame\Saved\Config\WindowsServer\` per [asa-process-runbook.md](asa-process-runbook.md) |
| `mods\` | Mod content (MVP may be empty or manually seeded) |
| `logs\` | Manager-captured game stdout/stderr and manager job logs |
| `backups\` | Archives produced by backup jobs |

## Naming

- `serverId`: opaque lowercase identifier (UUID without braces, or nanoid — **implementation detail** must be fixed in code constants doc; planning assumes **UUID v4 string lowercase**).

## Permissions intent

- Service account running the manager must have **Modify** ACL on `ManagerDataRoot` and descendants.
- Operators should **not** share the service account with untrusted interactive users on the same host; threat model assumes server-class single-tenant or controlled admin access.

## Backup boundary

- Default backup job archives at minimum: `binaries\ShooterGame\Saved\Config\WindowsServer\**` and `binaries\ShooterGame\Saved\SavedArks\**` per [asa-process-runbook.md](asa-process-runbook.md) and [backup-mvp.md](../requirements/backup-mvp.md); include `configs\**` if used.

## Sources

- [ADR-0001](../decisions/ADR-0001-host-and-deployment.md)
- [product/00-foundations.md](../../product/00-foundations.md)
