# How to evaluate planning completeness

Use this checklist **before** generating application code or opening a “build MVP” epic.

## A. Gate table (authoritative)

Work through [meta/how-we-plan.md](../../meta/how-we-plan.md) **Pre-code gate status**. Every gate must be **done** or **waived** with ADR linkage.

## B. Self-contained task test

Pick a random **Phase C*** row from [implementation-backbone.md](implementation-backbone.md). Ask:

1. Can an implementer find **all** inputs in `plan/` without chat?  
2. Is **done when** objectively testable?  
3. Are **out-of-scope** items explicitly excluded in [mvp-scope.md](../requirements/mvp-scope.md)?

If any answer is “no”, return to `open-questions.md` or add an ADR.

## C. Contradiction scan (15 minutes)

- Search for “Linux” in `plan/wiki` and `plan/product` — must only appear as **non-goal** or historical note.
- Search for “plugin” — must align with ADR-0002 deferred wording.
- Verify Steam app IDs: dedicated **2430930** vs client **2399830** — see [raw/asa-steamcmd-notes.md](../../raw/asa-steamcmd-notes.md).

## D. API + persistence specs

- [api-v1-mvp.md](../requirements/api-v1-mvp.md) checklist complete vs intended routes.
- [persistence-mvp.md](persistence-mvp.md) + [persistence-providers.md](persistence-providers.md) describe MVP tables and Sqlite/Postgres switch.

## E. ASA pitfalls coverage

- [asa-known-issues.md](../risks/asa-known-issues.md) matrix reviewed; mitigations map to jobs/runbook.

## F. Risk acceptance

- Sign off [threat-model-stub.md](../risks/threat-model-stub.md) mitigations with named owner (person or role).

## Sources

- [meta/how-we-plan.md](../../meta/how-we-plan.md)
- [open-questions.md](../open-questions.md)
