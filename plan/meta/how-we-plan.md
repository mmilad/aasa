# How we plan (meta-plan)

This document is the **single source of truth** for how planning work runs until ambiguity is removed, and how the `plan/` tree stays healthy.

## Three layers (Karpathy-style)

| Layer | Location | Role |
|-------|----------|------|
| Raw sources | `plan/raw/` | Immutable inputs: links, excerpts, legacy snapshots. Agents read; do not rewrite to “fix truth.” |
| Compiled wiki + product | `plan/wiki/`, `plan/product/` | Synthesis, requirements, ADRs, modular specs. Contradictions resolved here with traceability. |
| Schema | `plan/WIKI_SCHEMA.md`, `.cursor/rules/ark-planning.mdc` | Conventions: ADR shape, provenance, index/log/done maintenance. |

Reference: [LLM Wiki (gist)](https://gist.github.com/karpathy/442a6bf555914893e9891c11519de94f).

---

## Intake pipeline (operational workflow)

1. **Intake** — New material arrives (doc URL, experiment note, screenshot description). Add under `plan/raw/` or append [sources.bib.md](../raw/sources.bib.md); append [log.md](../log.md) with an `ingest` entry.
2. **Triage** — Classify: requirement, constraint, decision, risk, or open question.
3. **Synthesize** — Update or create `plan/wiki/*` pages; link related product bullets in `plan/product/*`.
4. **Decide** — If tradeoffs exist, add or update an ADR under `plan/wiki/decisions/` (see [WIKI_SCHEMA.md](../WIKI_SCHEMA.md) template).
5. **De-queue questions** — Every row in [open-questions.md](../wiki/open-questions.md) must close as:
   - **ADR** (accepted) + links from product spec, or
   - **Spec text** in `plan/product/` with enough detail to implement, or
   - **Explicit deferral**: reason + **revisit trigger** (date, milestone, or external event). Deferred items stay in the table with `deferred` status.
6. **Evidence in done** — When a planning task closes, add a row to [done.md](../done.md) with links to ADR/spec/log.

---

## Pre-code ambiguity gate (implementation blocked until ALL satisfied)

Application/runtime **code generation** stays out of scope until every item below is checked **done** with evidence linked from [done.md](../done.md) or from the ADR/spec that satisfies the item.

| # | Gate | Evidence expectation |
|---|------|------------------------|
| G1 | **MVP scope** paragraph exists and is **falsifiable** (includes explicit **non-goals**) | `plan/wiki/` or `plan/product/` + link in index |
| G2 | **Host OS targets** and **deployment shape** chosen (Windows-only, Linux server, or both) and consequences stated | [ADR-0001](../wiki/decisions/ADR-0001-host-and-deployment.md) status `accepted` |
| G3 | **Threat model stub** for control API + auth + RCON exposure (who can attack what surface) | [threat-model-stub.md](../wiki/risks/threat-model-stub.md) filled beyond placeholders, or ADR section |
| G4 | **Data directory contract** frozen (layout intent, permissions, backup boundaries) | `plan/product/00-foundations.md` or wiki architecture page |
| G5 | **SteamCMD + ASA dedicated** assumptions documented with **pinned references** in `plan/raw/` (e.g. sources.bib + excerpts) | [sources.bib.md](../raw/sources.bib.md) + optional raw excerpt file |
| G6 | **Plugin model** chosen at contract level (transport, auth, isolation) **or** explicitly deferred with revisit trigger | [ADR-0002](../wiki/decisions/ADR-0002-plugin-integration.md) `accepted` or `deferred` per schema rules |
| G7 | **Secrets strategy** chosen (storage, rotation, logs redaction) consistent with API exposure | [ADR-0003](../wiki/decisions/ADR-0003-secrets-and-exposure.md) `accepted` or linked deferral |

When the **Gate status** table below shows any item not `done`, [README.md](../README.md) should treat **implementation work as out of scope** until evidence is restored.

### Pre-code gate status (snapshot 2026-04-27)

| Gate | Status | Evidence |
|------|--------|----------|
| G1 | done | [mvp-scope.md](../wiki/requirements/mvp-scope.md) |
| G2 | done | [ADR-0001](../wiki/decisions/ADR-0001-host-and-deployment.md) **accepted** |
| G3 | done | [threat-model-stub.md](../wiki/risks/threat-model-stub.md) (MVP sections filled) |
| G4 | done | [data-layout-windows.md](../wiki/architecture/data-layout-windows.md) |
| G5 | done | [asa-steamcmd-notes.md](../raw/asa-steamcmd-notes.md) + [sources.bib.md](../raw/sources.bib.md) |
| G6 | done | [ADR-0002](../wiki/decisions/ADR-0002-plugin-integration.md) **deferred** with revisit trigger |
| G7 | done | [ADR-0003](../wiki/decisions/ADR-0003-secrets-and-exposure.md) **accepted** |

Human policy: treat this table as **ready for implementation** only when you personally agree the snapshot matches operational reality; update the date row when anything changes.

---

## Ultrapanning phases (planning-only)

Use these phases to drive ambiguity to zero **before** opening `implementation-backbone.md` coding steps.

| Phase | Goal | Exit criteria |
|-------|------|----------------|
| **U0** | Stack + host frozen | ADR-0001 accepted; [tech-stack.md](../wiki/architecture/tech-stack.md) exists |
| **U1** | MVP falsifiable | [mvp-scope.md](../wiki/requirements/mvp-scope.md) lists in-scope + **non-goals** + binary success criteria |
| **U2** | Security + data contract | ADR-0003 accepted; data layout doc; threat model mitigations listed |
| **U3** | Install pipeline pinned | Steam app ID + example SteamCMD flow in `raw/`; product lifecycle links |
| **U4** | Extensions stance | ADR-0002 deferred **or** accepted (no limbo) |
| **U5** | Execution backbone | [implementation-backbone.md](../wiki/implementation-backbone.md) + [evaluation.md](../wiki/architecture/evaluation.md) complete |

## Do you need Cursor “Plan mode”?

**No.** Plan mode is useful when you want **discussion-only** (no repo edits). For systematically updating `plan/**`, **Agent mode** (normal editing) is appropriate. Use Plan mode when you prefer to agree on direction before any files change.

---

## Lint cadence (wiki health)

Run on a **weekly** cadence or before any “start coding” milestone:

- **Contradictions** — Two documents assert incompatible facts; resolve via ADR and add supersession pointers on stale pages.
- **Orphans** — Pages under `wiki/` or `product/` not reachable from [index.md](../index.md); fix links or add index rows.
- **Stale claims** — Newer ADR supersedes narrative; top-of-file pointer on old page.
- **Missing sources** — Non-obvious wiki claims without **Sources** footer; add links to `raw/` or external URLs, or mark draft.
- **Open questions hygiene** — No row left `open` without a next action owner; deferrals have triggers.

Append a `lint` entry to [log.md](../log.md) when a pass completes, with date and notable fixes.

---

## Definitions

- **Ambiguity resolved** — For a given topic, a reader can implement or procure infrastructure without clarifying questions; remaining work is ordinary engineering risk, not unknown requirements.
- **Planning task** — A discrete outcome (e.g. “accept ADR-0001”) recorded in [done.md](../done.md).

See also [glossary.md](glossary.md).
