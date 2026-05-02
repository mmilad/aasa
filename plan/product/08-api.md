# Product: API layer

Migrated from the former consolidated implementation outline.

---

## 8. API layer

- REST endpoints
  - servers
  - mods
  - jobs
  - players
- WebSocket
  - logs
  - status
  - metrics

**MVP v0.1:** normative contract — [api-v1-mvp.md](../wiki/requirements/api-v1-mvp.md) (route table, errors, auth). Auth per [ADR-0003](../wiki/decisions/ADR-0003-secrets-and-exposure.md). First-party **Blazor** admin consumes this API per [ADR-0004](../wiki/decisions/ADR-0004-local-admin-ui-blazor.md) (Phase C5 in [implementation-backbone.md](../wiki/implementation-backbone.md)). Broader surface (mods/jobs/players WebSocket) rolls in post–Phase C3 per backbone.

---

## Related

- Previous: [07-backups.md](07-backups.md)
- Next: [09-ui.md](09-ui.md)
- [ADR-0003: Secrets and exposure](../wiki/decisions/ADR-0003-secrets-and-exposure.md)
