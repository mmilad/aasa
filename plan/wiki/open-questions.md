# Open questions

Every item in **Queue** MUST end as: an **ADR**, **spec text** in `plan/product/`, or **explicit deferral** (reason + revisit trigger).

## Queue

| ID | Question | Owner hint | Status |
|----|----------|------------|--------|
| _none_ | — | — | — |

_Add new rows when unknowns appear during implementation or research._

## Resolved / closed (archive)

| ID | Resolution | Links |
|----|--------------|-------|
| OQ-001 | Manager + stack: **Windows Server**, **C# / .NET 8**, ASA Windows dedicated | [ADR-0001](decisions/ADR-0001-host-and-deployment.md), [tech-stack.md](architecture/tech-stack.md) |
| OQ-002 | Plugins **deferred** with explicit revisit trigger | [ADR-0002](decisions/ADR-0002-plugin-integration.md) |
| OQ-003 | Secrets + bind + API key auth | [ADR-0003](decisions/ADR-0003-secrets-and-exposure.md) |
| OQ-004 | Steam dedicated **2430930** vs client **2399830** pinned | [raw/asa-steamcmd-notes.md](../raw/asa-steamcmd-notes.md), [sources.bib.md](../raw/sources.bib.md), [product/01-lifecycle.md](../product/01-lifecycle.md) |
| OQ-005 | MVP scope + non-goals + success criteria | [mvp-scope.md](requirements/mvp-scope.md) |

### Deferred row template

| ID | Question | Status | Deferral reason | Revisit trigger |
|----|----------|--------|-----------------|-----------------|
| _example_ | _question_ | deferred | _why now is not the time_ | _e.g. after MVP ship / when API stable_ |

_Append closure notes to [log.md](../log.md) and [done.md](../done.md) when moving rows here._
