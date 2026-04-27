# Agent execution conventions (normative pages)

## Purpose

Planning artifacts are written primarily for **autonomous agent implementation**. This file defines the **minimum structure** of any normative `plan/wiki/**` spec (requirements, architecture slice, risk mitigations).

## Required sections

1. **Purpose** — one short paragraph: what decision this doc freezes.
2. **Normative rules** — bullets or tables: MUST / MUST NOT.
3. **Agent acceptance checklist** — `- [ ]` items that are **pass/fail** without subjective judgment.
4. **Sources** — links to `plan/raw/`, ADRs, or external URLs for non-obvious facts.

## Optional but encouraged

- **Pseudocode** blocks for orchestration (SteamCMD, stop/start).
- **Explicit out-of-scope** bullets pointing at [mvp-scope.md](../requirements/mvp-scope.md) or ADRs.

## Sources

- [WIKI_SCHEMA.md](../../WIKI_SCHEMA.md)
- [meta/how-we-plan.md](../../meta/how-we-plan.md)
