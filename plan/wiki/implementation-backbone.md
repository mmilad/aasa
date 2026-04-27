# Implementation backbone (single reading order for coding)

When implementation starts, follow this **order** so each step has **inputs** (docs) and **done** criteria without re-deriving architecture from chat.

## Before any code

1. Confirm [meta/how-we-plan.md](../../meta/how-we-plan.md) **Gate status** shows all gates satisfied (or explicitly waived with ADR).
2. Read [tech-stack.md](tech-stack.md), [data-layout-windows.md](data-layout-windows.md), [mvp-scope.md](../requirements/mvp-scope.md), [persistence-providers.md](architecture/persistence-providers.md), [agent-execution-conventions.md](meta/agent-execution-conventions.md).
3. Read ADRs: [ADR-0001](../decisions/ADR-0001-host-and-deployment.md), [ADR-0003](../decisions/ADR-0003-secrets-and-exposure.md), [ADR-0002](../decisions/ADR-0002-plugin-integration.md) (deferred scope).

## Phase C0 — Solution skeleton (first code milestone)

| Step | Do | Done when |
|------|-----|-----------|
| C0.1 | Create solution: Worker Service + ASP.NET Core hosting | `dotnet build` succeeds; installs as Windows Service doc stub exists in `plan/product/12-deployment.md` appendix |
| C0.2 | Config + logging wired | Settings load from JSON; logs land under `%ProgramData%\ArkServerManager\logs\` or configurable |
| C0.3 | Health endpoint | `GET /health` returns 200 with version/build |

## Phase C1 — Data + server registry

| Step | Do | Done when |
|------|-----|-----------|
| C1.1 | EF schema per [persistence-mvp.md](architecture/persistence-mvp.md) | Migrations apply on Sqlite dev; repositories only; matches [product/00-foundations.md](../../product/00-foundations.md) MVP fields |
| C1.2 | Create server allocates disk tree | Folders exist per [data-layout-windows.md](data-layout-windows.md) |

## Phase C2 — SteamCMD + lifecycle

| Step | Do | Done when |
|------|-----|-----------|
| C2.1 | SteamCMD path config + `app_update 2430930` job | Logs streamed; exit code captured; `409` if server not stopped ([jobs-mvp.md](../requirements/jobs-mvp.md)); hints per [asa-known-issues.md](../risks/asa-known-issues.md) |
| C2.2 | Start/stop process | Paths per [asa-process-runbook.md](architecture/asa-process-runbook.md); PID persisted; stop per [rcon-mvp.md](../requirements/rcon-mvp.md) then kill |

## Phase C3 — API surface for MVP

| Step | Do | Done when |
|------|-----|-----------|
| C3.1 | OpenAPI + implementation match [api-v1-mvp.md](../requirements/api-v1-mvp.md) route table | Every row implemented; acceptance checklist in that file 100% checked |
| C3.2 | Auth middleware | API key header enforced per ADR-0003 |

## Phase C4 — INI + backups (MVP tail)

| Step | Do | Done when |
|------|-----|-----------|
| C4.1 | INI read/write with backup | Matches [product/06-config-ini.md](../../product/06-config-ini.md) allowlist |
| C4.2 | Backup zip job | [backup-mvp.md](../requirements/backup-mvp.md) + `POST .../jobs/backup` |

## After C4 (hardening)

Follow [production-readiness.md](../requirements/production-readiness.md) for PR-* phases before calling the product “production ready.”

_Add phases C5+ only via ADR or mvp-scope revision._

## Sources

- [mvp-scope.md](../requirements/mvp-scope.md)
- [architecture/evaluation.md](architecture/evaluation.md)
