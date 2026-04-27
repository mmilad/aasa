# Production readiness (beyond MVP v0.1)

## Purpose

**MVP v0.1** can ship when [mvp-scope.md](mvp-scope.md) success criteria pass. **“Production ready”** means an **explicit subset** of the phases below is checked off for your deployment—define that subset per environment (e.g. homelab vs hosted).

## PR-1 — Engineering hygiene

- [ ] Unit test projects for domain + API; integration tests for DB + SteamCMD fakes/mocks.
- [ ] CI on every PR (`dotnet test`, `dotnet format` optional).
- [ ] Versioning policy (SemVer tags, changelog).

## PR-2 — Release and ops

- [ ] Publish artifact: **zip + `sc create` instructions** or MSI—pick one and document.
- [ ] **Provider switch**: SQLite file backup before migrating to Postgres; Postgres backup/restore runbook ([persistence-providers.md](../architecture/persistence-providers.md)).
- [ ] Upgrade: migrate DB schema on startup or documented `dotnet ef` step.

## PR-3 — Reliability

- [ ] Structured logs + correlation id per HTTP request and per job.
- [ ] Disk-space preflight before SteamCMD ([asa-known-issues.md](../risks/asa-known-issues.md)).
- [ ] Service restart recovery: jobs `Running` → `Failed` or resumed with idempotency rules.

## PR-4 — Security hardening

- [ ] TLS termination story when `Listen:Public` is used.
- [ ] Secret rotation playbook (API key, RCON).
- [ ] Dependency audit cadence (`dotnet list package --vulnerable`).

## PR-5 — Observability

- [ ] Optional `/metrics` (Prometheus) behind feature flag—only if ADR accepts scope.

## Definition gate

Before marketing “production ready,” pick **minimum** checkboxes from PR-1–PR-4 (e.g. CI + backups + TLS story) and record them in [done.md](../../done.md) with owner sign-off.

## Sources

- [implementation-backbone.md](../implementation-backbone.md)
- [evaluation.md](../architecture/evaluation.md)
