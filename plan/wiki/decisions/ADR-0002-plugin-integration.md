# ADR-0002: External plugin integration

- **Status**: deferred  
- **Date**: 2026-04-27  

## Context

The long-term vision includes **self-hosted external plugins** and **schema-driven UI** ([product/extensibility-ui-plugins.md](../../product/extensibility-ui-plugins.md)). Designing transport, auth, and CSP for hosted panels **before** a working core would delay MVP without reducing core risk.

Tracks: [open-questions.md](../open-questions.md) **OQ-002** — closed as **deferred** (not abandoned).

## Decision (MVP)

**Defer** any externally callable plugin protocol, extension registry, or UI contribution surface **until after** the following **revisit trigger** is met:

- **Revisit trigger**: MVP v0.1 shipped **and** `/api/v1` routes used in production for **one week** without breaking changes **and** ADR review scheduled for “extensions v0.2”.

Until then:

- The core ships **without** plugin HTTP/gRPC listeners beyond what the core itself needs.  
- Product text may describe future **manifest + capability** model, but no wire protocol is guaranteed stable pre-trigger.

## Consequences

- Positive: Faster MVP; smaller attack surface; fewer moving parts while learning .NET services.  
- Negative: Early adopters cannot extend via first-class plugins until phase 2; potential rework if extension API is rushed later — mitigated by keeping extension doc as **non-normative** until ADR accepted.

## Alternatives considered

- **Ship HTTP sidecar plugin API in MVP** — rejected: expands threat model and documentation burden before core lifecycle works.  
- **Webhooks-only plugins** — deferred alongside general plugin ADR; may re-enter as smaller first step at revisit.

## Sources

- [mvp-scope.md](../requirements/mvp-scope.md)
- [product/extensibility-ui-plugins.md](../../product/extensibility-ui-plugins.md)
