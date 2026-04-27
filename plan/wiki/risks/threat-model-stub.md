# Threat model — control plane (MVP)

Pre-code gate **G3**. Mitigations align with [ADR-0003](../decisions/ADR-0003-secrets-and-exposure.md) and deployment assumptions in [ADR-0001](../decisions/ADR-0001-host-and-deployment.md).

## Assets

| Asset | Impact if lost |
|-------|------------------|
| Manager API availability | Operator cannot administer servers |
| Manager API integrity | Attacker could start/stop processes or alter INI |
| RCON / Steam / API credentials | Game server compromise or unauthorized admin |
| `ManagerDataRoot` filesystem | Data destruction, malicious mod/binary drop |

## Actors

| Actor | Trust |
|-------|--------|
| Operator (RDP / local admin) | Trusted |
| Same-host other Windows users | **Untrusted** if interactive logons share machine — MVP assumes server-class separation |
| Remote attacker on network | **In scope** when API listens beyond loopback |

## Surfaces

| Surface | Exposure (MVP default) | Notes |
|---------|------------------------|--------|
| REST / WebSockets | **Loopback only** | Public bind opt-in per ADR-0003 |
| SQLite DB + config files | Local disk | ACLs required on `ManagerDataRoot` |
| SteamCMD subprocess | Child process | Inherits service account; command line must not log secrets |
| Plugin HTTP | **Absent** | ADR-0002 deferred — no listener |

## Threat scenarios and mitigations

| # | Threat | Mitigation |
|---|--------|------------|
| T1 | Local malware reads API key file | NTFS ACL to service + admins; file outside repo |
| T2 | Operator accidentally exposes API on `0.0.0.0` | Default loopback; explicit opt-in + warning |
| T3 | Secrets in logs | Redaction rules + tests (ADR-0003) |
| T4 | RCON brute force from game network | Out of manager scope; document firewall guidance in deployment doc |

## Residual risks (accepted for MVP)

- No built-in rate limiting on API (localhost trust).  
- SQLite file not encrypted at rest by default (ACL-only).

## Sources

- [ADR-0003](../decisions/ADR-0003-secrets-and-exposure.md)
- [ADR-0001](../decisions/ADR-0001-host-and-deployment.md)
- [product/11-security.md](../../product/11-security.md)
- [meta/how-we-plan.md](../../meta/how-we-plan.md)
