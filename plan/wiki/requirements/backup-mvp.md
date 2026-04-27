# Backups — MVP

## Purpose

Minimum crash-safe artifact for configs + saves path aligned with [data-layout-windows.md](../architecture/data-layout-windows.md) and [asa-process-runbook.md](../architecture/asa-process-runbook.md).

## MVP scope

- **Trigger**: `POST /api/v1/servers/{serverId}/jobs/backup` per [api-v1-mvp.md](api-v1-mvp.md); job type `Backup` in [persistence-mvp.md](../architecture/persistence-mvp.md). **On-demand zip** in milestone C4.2.
- **Archive includes**:
  - `{server}/configs/**` if files mirrored there; else include **`ShooterGame\Saved\Config\WindowsServer\`** under install root
  - **`ShooterGame\Saved\SavedArks\`** under install root (world saves)
- **Retention / schedule**: **out of MVP** — no cron; single zip per manual action.

## Restore

- **Out of MVP** for automated restore. Operator procedure: stop server, unzip into paths documented in [operator runbook](../../product/12-deployment.md), verify, start.

## Agent acceptance checklist

- [ ] Zip opens on Windows without third-party tools (built-in explorer).
- [ ] Contains at least one `.ini` from WindowsServer config path and at least one save under `SavedArks` when saves exist.

## Sources

- [product/07-backups.md](../../product/07-backups.md)
- [mvp-scope.md](mvp-scope.md)
