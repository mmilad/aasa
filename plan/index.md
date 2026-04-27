# Plan index (content catalog)

LLM-maintained catalog: each substantive page, one-line summary, category. Update when adding pages under `wiki/` or `product/`.

## Hub

| Page | Summary | Category |
|------|---------|----------|
| [README.md](README.md) | Entry map and reading order | hub |
| [WIKI_SCHEMA.md](WIKI_SCHEMA.md) | Conventions + agent-execution norms pointer | schema |

## Meta

| Page | Summary | Category |
|------|---------|----------|
| [meta/how-we-plan.md](meta/how-we-plan.md) | Gates, ultraplan, intake | meta |
| [meta/glossary.md](meta/glossary.md) | Terminology | meta |

## Wiki — meta / execution

| Page | Summary | Category |
|------|---------|----------|
| [wiki/meta/agent-execution-conventions.md](wiki/meta/agent-execution-conventions.md) | Required sections for normative specs | meta |
| [wiki/implementation-backbone.md](wiki/implementation-backbone.md) | Coding phases C0–C4 + after C4 | wiki |

## Wiki — requirements

| Page | Summary | Category |
|------|---------|----------|
| [wiki/requirements/mvp-scope.md](wiki/requirements/mvp-scope.md) | MVP scope, non-goals, success criteria | requirements |
| [wiki/requirements/api-v1-mvp.md](wiki/requirements/api-v1-mvp.md) | REST v1 route table + errors + checklist | requirements |
| [wiki/requirements/jobs-mvp.md](wiki/requirements/jobs-mvp.md) | Job types, concurrency, ports | requirements |
| [wiki/requirements/rcon-mvp.md](wiki/requirements/rcon-mvp.md) | RCON timeouts, shutdown, logging | requirements |
| [wiki/requirements/backup-mvp.md](wiki/requirements/backup-mvp.md) | Backup zip MVP + restore policy | requirements |
| [wiki/requirements/production-readiness.md](wiki/requirements/production-readiness.md) | PR-* production phases | requirements |

## Wiki — architecture

| Page | Summary | Category |
|------|---------|----------|
| [wiki/architecture/tech-stack.md](wiki/architecture/tech-stack.md) | .NET 8, EF, services | architecture |
| [wiki/architecture/data-layout-windows.md](wiki/architecture/data-layout-windows.md) | Directory contract | architecture |
| [wiki/architecture/persistence-mvp.md](wiki/architecture/persistence-mvp.md) | MVP tables + enums | architecture |
| [wiki/architecture/persistence-providers.md](wiki/architecture/persistence-providers.md) | Sqlite / Postgres switch | architecture |
| [wiki/architecture/asa-process-runbook.md](wiki/architecture/asa-process-runbook.md) | Exe, cwd, launch, stop | architecture |
| [wiki/architecture/evaluation.md](wiki/architecture/evaluation.md) | Planning completeness checks | architecture |

## Wiki — risks / overview / decisions

| Page | Summary | Category |
|------|---------|----------|
| [wiki/overview.md](wiki/overview.md) | Architecture snapshot | wiki |
| [wiki/open-questions.md](wiki/open-questions.md) | Queue + resolved archive | wiki |
| [wiki/risks/threat-model-stub.md](wiki/risks/threat-model-stub.md) | Threat model MVP | risks |
| [wiki/risks/asa-known-issues.md](wiki/risks/asa-known-issues.md) | ASA/SteamCMD pitfalls matrix | risks |
| [wiki/decisions/ADR-0001-host-and-deployment.md](wiki/decisions/ADR-0001-host-and-deployment.md) | Windows + C# accepted | adr |
| [wiki/decisions/ADR-0002-plugin-integration.md](wiki/decisions/ADR-0002-plugin-integration.md) | Plugins deferred | adr |
| [wiki/decisions/ADR-0003-secrets-and-exposure.md](wiki/decisions/ADR-0003-secrets-and-exposure.md) | Loopback + API key | adr |

## Product

| Page | Summary | Category |
|------|---------|----------|
| [product/00-foundations.md](product/00-foundations.md) | Runtime, models, persistence links | product |
| [product/01-lifecycle.md](product/01-lifecycle.md) | Lifecycle + Steam + runbook links | product |
| [product/02-updates-automation.md](product/02-updates-automation.md) | Scheduler (MVP jobs ref) | product |
| [product/03-mods.md](product/03-mods.md) | Mods | product |
| [product/04-players-rcon.md](product/04-players-rcon.md) | RCON / players + rcon-mvp | product |
| [product/05-monitoring.md](product/05-monitoring.md) | Monitoring | product |
| [product/06-config-ini.md](product/06-config-ini.md) | INI paths + allowlist MVP | product |
| [product/07-backups.md](product/07-backups.md) | Backups + backup-mvp | product |
| [product/08-api.md](product/08-api.md) | API layer + api-v1-mvp | product |
| [product/09-ui.md](product/09-ui.md) | UI optional | product |
| [product/10-integrations.md](product/10-integrations.md) | Integrations | product |
| [product/11-security.md](product/11-security.md) | Security | product |
| [product/12-deployment.md](product/12-deployment.md) | Windows Service + operator checklist | product |
| [product/13-advanced.md](product/13-advanced.md) | Advanced | product |
| [product/extensibility-ui-plugins.md](product/extensibility-ui-plugins.md) | Extensibility | product |

## Raw

| Page | Summary | Category |
|------|---------|----------|
| [raw/README.md](raw/README.md) | Raw policy | raw |
| [raw/sources.bib.md](raw/sources.bib.md) | URL bibliography | raw |
| [raw/asa-steamcmd-notes.md](raw/asa-steamcmd-notes.md) | SteamCMD app ids | raw |
| [raw/asa-install-layout-windows.md](raw/asa-install-layout-windows.md) | ShooterGame paths | raw |
| [raw/legacy-ark-server-manager-plan-monolith.md](raw/legacy-ark-server-manager-plan-monolith.md) | Legacy monolith | raw |
