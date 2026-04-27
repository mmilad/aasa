# Product: Config management

Migrated from the former consolidated implementation outline. **MVP paths and API** are normative below.

---

## 6. Config management

- Parse INI ↔ JSON for API consumers
- Edit via API (MVP allowlist only)
- Validate inputs
- Write back to INI with **backup-before-write** (`.bak` timestamp)
- Never log secret values at default log level

---

## MVP — canonical paths (Windows)

Let `B` = server `install_root` = `{ManagerDataRoot}\servers\{serverId}\binaries` (see [data-layout-windows.md](../wiki/architecture/data-layout-windows.md)).

| File | Full path |
|------|-----------|
| `GameUserSettings.ini` | `B\ShooterGame\Saved\Config\WindowsServer\GameUserSettings.ini` |
| `Game.ini` | `B\ShooterGame\Saved\Config\WindowsServer\Game.ini` |

Source excerpt: [raw/asa-install-layout-windows.md](../raw/asa-install-layout-windows.md).

## MVP — API allowlist (PATCH)

Only these keys may be mutated via `PATCH .../ini/GameUserSettings` in v0.1 (expand later via ADR):

| Section | Key | Purpose |
|---------|-----|---------|
| `ServerSettings` | `ServerPassword` | satisfy MVP “one key change” test (may be empty string) |
| `ServerSettings` | `ServerAdminPassword` | align with RCON admin password if operator chooses |
| `ServerSettings` | `MaxPlayers` | simple int validation |

**Rule:** any other key returns **`400 VALIDATION_FAILED`** with `code: KEY_NOT_ALLOWLISTED`.

## Backup-before-write

Before writing INI: copy file to same directory with suffix `.bak.yyyyMMddHHmmss`.

---

## Related

- Previous: [05-monitoring.md](05-monitoring.md)
- Next: [07-backups.md](07-backups.md)
- [api-v1-mvp.md](../wiki/requirements/api-v1-mvp.md)
- [asa-process-runbook.md](../wiki/architecture/asa-process-runbook.md)
- [extensibility-ui-plugins.md](extensibility-ui-plugins.md)
