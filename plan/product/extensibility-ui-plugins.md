# Product: Extensibility — schema-driven UI and external plugins

> **MVP:** external plugin wire protocol is **deferred** — see [ADR-0002](../wiki/decisions/ADR-0002-plugin-integration.md). This file remains the **design north star** for post-MVP work.

This module captures **cross-cutting** extensibility not fully spelled out in the original monolith. It complements [09-ui.md](09-ui.md), [06-config-ini.md](06-config-ini.md), [08-api.md](08-api.md), and [13-advanced.md](13-advanced.md).

## Goals

- **Reusable UI** driven by configuration/schema rather than one-off screens per setting.
- **Self-hosted external plugins** that integrate without forking core, via a documented contract.

## Schema-driven UI (contract-level)

- Represent server, cluster, and mod settings as **machine-readable schemas** (e.g. JSON Schema or equivalent) with UI metadata: labels, grouping, validation, “requires restart” flags.
- Core ships a **generic form renderer** + composed layouts (dashboard shell, server detail slots, log panel).
- New settings fields primarily extend the schema; bespoke UI is the exception.

**Open**: exact schema format and versioning — see [ADR-0002](../wiki/decisions/ADR-0002-plugin-integration.md) and [open-questions.md](../wiki/open-questions.md).

## External self-hosted plugins (contract-level)

- Plugins are **out-of-process** unless an ADR explicitly allows in-process loading.
- Core exposes a **stable API surface** (REST and/or gRPC) and a **manifest** model: id, version, required capabilities, declared permissions/scopes.
- Authentication between core and plugins: **token or mTLS** (decision in ADR-0002 / ADR-0003).
- Failure isolation: plugin outage must not take down core; degraded mode behavior must be specified per capability.

## UI contribution model (options to decide in ADR-0002)

- **Metadata only**: plugin supplies schema fragments + links; core renders.
- **Hosted surface**: plugin provides a URL for an embedded panel with strict CSP (tradeoffs: XSS, operational burden).

## Related

- [Open questions](../wiki/open-questions.md)
- [ADR-0002: Plugin integration](../wiki/decisions/ADR-0002-plugin-integration.md)
- [ADR-0003: Secrets and exposure](../wiki/decisions/ADR-0003-secrets-and-exposure.md)
- [WIKI_SCHEMA.md](../WIKI_SCHEMA.md) (documentation provenance for planning itself)
