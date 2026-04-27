# ADR-0003: Secrets and network exposure (MVP)

- **Status**: accepted  
- **Date**: 2026-04-27  

## Context

The manager stores and uses sensitive values: **RCON password**, optional **Steam** credentials for SteamCMD (prefer `login anonymous` when possible), **API keys** for the control plane, and future integration tokens. Logs and crash dumps must not leak secrets by default.

Tracks (resolved): ~~[open-questions.md](../open-questions.md) OQ-003~~.

## Decision (MVP v0.1)

1. **Network bind**: Kestrel defaults to **`127.0.0.1` only** (loopback). Binding to LAN interfaces is **opt-in** via explicit configuration flag `Listen:Public` (name illustrative) and must log a **high-severity warning** on startup when enabled.  
2. **API authentication**: A single **shared API key** presented as HTTP header `X-Api-Key` (exact header name fixed in OpenAPI at implementation). Requests without a valid key receive **401**.  
3. **API key storage**: Stored in a **JSON config file outside the repository** (e.g. next to the service binary or under `ManagerDataRoot\secrets\`) with **NTFS ACL** restricting read to the service account + administrators only. **DPAPI optional enhancement** post-MVP unless trivial to add early.  
4. **RCON / Steam secrets**: Stored in SQLite **encrypted-at-rest** is **post-MVP** unless a small library is chosen during implementation spike; **MVP** requirement is: values are **not** in source control, **not** in default info-level logs, and DB file is ACL-protected like the API key file.  
5. **Logging**: Default log level **must not** print secrets; add automated test or checklist item to redact known key patterns (implementation task).

## Consequences

- Positive: Simple mental model for a solo admin on Windows Server; small attack surface when loopback default is kept.  
- Negative: Remote administration requires RDP / SSH tunnel / reverse proxy setup by operator — acceptable for MVP per [mvp-scope.md](../requirements/mvp-scope.md).

## Alternatives considered

- **mTLS for localhost plugins** — unnecessary while ADR-0002 deferred.  
- **TLS + bearer tokens for LAN** — post-MVP; revisit when `Listen:Public` is used in field deployments.  
- **Windows Credential Manager for all secrets** — attractive; deferred as optional hardening after MVP stability.

## Sources

- [threat-model-stub.md](../risks/threat-model-stub.md)
- [mvp-scope.md](../requirements/mvp-scope.md)
