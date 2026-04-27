# Wiki schema (agent configuration)

This document is the **schema layer** for the `plan/` tree: it tells humans and agents how knowledge is structured, updated, and proven.

## Layers

1. **`plan/raw/`** — Immutable inputs. Add new material here; do not “edit away” primary sources. Corrections go in `wiki/` or ADRs with a note that supersedes a given raw excerpt.
2. **`plan/wiki/`** — Compiled, cross-linked synthesis (requirements, architecture, ADRs, risks).
3. **`plan/product/`** — Modular implementation-oriented specs. May reference wiki ADRs.

## File naming

- ADRs: `plan/wiki/decisions/ADR-NNNN-short-kebab-title.md` (four-digit `NNNN`, zero-padded).
- Product modules: `plan/product/NN-topic.md` (two-digit prefix).

## ADR template (required sections)

Each ADR MUST include:

- **Status**: proposed | accepted | superseded | deprecated | deferred (if superseded, link to replacement ADR; if **deferred**, include revisit trigger in Context).
- **Context** — forces at play.
- **Decision** — what we chose.
- **Consequences** — positive and negative.
- **Alternatives considered** — at least one rejected option with reason.

## Provenance

- Non-obvious factual claims in `plan/wiki/**` SHOULD end with a **Sources** bullet list linking to `plan/raw/` files or external URLs.
- Until sourced, mark page top: `> Draft: pending primary sources.`

Normative specs SHOULD follow [wiki/meta/agent-execution-conventions.md](wiki/meta/agent-execution-conventions.md) (Purpose, rules, acceptance checklist, Sources).

## Agent maintenance rules (when editing `plan/**`)

1. Read [README.md](README.md) then this file before large edits.
2. After adding a substantive new page under `wiki/` or `product/`, update [index.md](index.md) (one-line summary + category).
3. After a material planning session or ingest, append [log.md](log.md) with the header format defined there.
4. When a planning task is **finished** (deliverable merged, question resolved with ADR/spec text), add an entry to [done.md](done.md) with links to the closing evidence.
5. Do not store secrets (API keys, passwords, RCON passwords) in `plan/`; reference env var names or secret store patterns only.

## Link conventions

- Prefer relative links from the current file’s directory (e.g. `../product/00-foundations.md`).
- Cross-link ADRs from product specs where a decision constrains implementation.

## Contradictions

If two documents disagree, **do not silently edit** the older narrative without a trace: add or update an ADR, then update the superseded doc with a one-line pointer at the top.
