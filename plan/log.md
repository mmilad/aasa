# Plan log (append-only)

Use consistent headers so entries are greppable, e.g. `## [YYYY-MM-DD] ingest | Short title`

---

## [2026-04-27] ingest | Planning scaffold and monolith migration

Initialized `plan/` tree (Karpathy-style layers), Cursor rule, migrated former root implementation outline into `plan/product/`, added ADR stubs and wiki overview. Implementation remains gated until pre-code checklist in `meta/how-we-plan.md` is complete.

## [2026-04-27] ingest | Raw legacy snapshot and product extensibility doc

Added immutable [raw/legacy-ark-server-manager-plan-monolith.md](raw/legacy-ark-server-manager-plan-monolith.md) and [product/extensibility-ui-plugins.md](product/extensibility-ui-plugins.md). Expanded [wiki/overview.md](wiki/overview.md) with module table and architecture sketch.

## [2026-04-27] session | Meta-plan gate checklist and lint cadence

Filled [meta/how-we-plan.md](meta/how-we-plan.md) with intake pipeline, pre-code gate G1–G7, lint cadence, and definitions. Updated [WIKI_SCHEMA.md](WIKI_SCHEMA.md) ADR statuses to include `deferred`. Aligned [wiki/open-questions.md](wiki/open-questions.md) deferral template.

## [2026-04-27] session | Ultrapanning — C#, MVP, gates satisfied in docs

Accepted **C# / .NET 8** + Windows Server baseline ([ADR-0001](wiki/decisions/ADR-0001-host-and-deployment.md)). Added [mvp-scope.md](wiki/requirements/mvp-scope.md), [data-layout-windows.md](wiki/architecture/data-layout-windows.md), [tech-stack.md](wiki/architecture/tech-stack.md), [implementation-backbone.md](wiki/implementation-backbone.md), [evaluation.md](wiki/architecture/evaluation.md). Pinned Steam dedicated **2430930** in [raw/asa-steamcmd-notes.md](raw/asa-steamcmd-notes.md). ADR-0002 **deferred**, ADR-0003 **accepted**. Emptied open-question queue into resolved archive. Gate status table populated in [meta/how-we-plan.md](meta/how-we-plan.md).

## [2026-04-27] session | Execute fill-MVP-planning-gaps plan (docs only)

Added [api-v1-mvp.md](wiki/requirements/api-v1-mvp.md), [persistence-mvp.md](wiki/architecture/persistence-mvp.md), [persistence-providers.md](wiki/architecture/persistence-providers.md), [jobs-mvp.md](wiki/requirements/jobs-mvp.md), [rcon-mvp.md](wiki/requirements/rcon-mvp.md), [backup-mvp.md](wiki/requirements/backup-mvp.md), [asa-process-runbook.md](wiki/architecture/asa-process-runbook.md), [asa-known-issues.md](wiki/risks/asa-known-issues.md), [raw/asa-install-layout-windows.md](raw/asa-install-layout-windows.md), [production-readiness.md](wiki/requirements/production-readiness.md), [agent-execution-conventions.md](wiki/meta/agent-execution-conventions.md). Updated product specs `00–08`, `12`, [implementation-backbone.md](wiki/implementation-backbone.md), [data-layout-windows.md](wiki/architecture/data-layout-windows.md), [evaluation.md](wiki/architecture/evaluation.md), [WIKI_SCHEMA.md](WIKI_SCHEMA.md), [mvp-scope.md](wiki/requirements/mvp-scope.md), [index.md](index.md).

## [2026-05-02] session | ADR-0004 — local Blazor admin UI + MVP scope alignment

Accepted [ADR-0004](wiki/decisions/ADR-0004-local-admin-ui-blazor.md) (Blazor Web App Interactive Server, separate project, loopback-first, no normative TS SPA). Updated [mvp-scope.md](wiki/requirements/mvp-scope.md) (in-scope item 8, non-goals, success criterion 5), [implementation-backbone.md](wiki/implementation-backbone.md) (Phase C5 after C4), [tech-stack.md](wiki/architecture/tech-stack.md), [product/08-api.md](product/08-api.md), [product/09-ui.md](product/09-ui.md), [index.md](index.md), [done.md](done.md).
