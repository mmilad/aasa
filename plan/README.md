# Planning hub (start here)

This repository uses a **multi-layer planning system** under `plan/`. **Pre-code gate checklist** in [meta/how-we-plan.md](meta/how-we-plan.md) is populated with evidence links; begin **application implementation** only after you acknowledge that snapshot and follow [wiki/implementation-backbone.md](wiki/implementation-backbone.md).

## Reading order

1. [WIKI_SCHEMA.md](WIKI_SCHEMA.md) — conventions, provenance, agent maintenance rules  
2. [meta/how-we-plan.md](meta/how-we-plan.md) — intake pipeline, gates, lint cadence  
3. [wiki/overview.md](wiki/overview.md) — synthesized product/architecture snapshot  
4. [wiki/requirements/mvp-scope.md](wiki/requirements/mvp-scope.md) — MVP in-scope / non-goals / success criteria  
5. [wiki/requirements/api-v1-mvp.md](wiki/requirements/api-v1-mvp.md) — REST route table (MVP)  
6. [wiki/architecture/persistence-providers.md](wiki/architecture/persistence-providers.md) — SQLite / PostgreSQL switch  
7. [wiki/implementation-backbone.md](wiki/implementation-backbone.md) — ordered coding phases (when implementation starts)  
8. [product/](product/) — modular implementation plan migrated from the former root monolith  
9. [wiki/open-questions.md](wiki/open-questions.md) — queue (empty when no unknowns) + resolved archive  
10. [index.md](index.md) — catalog of all substantive pages  

## Special files

| File | Role |
|------|------|
| [index.md](index.md) | Content catalog (one-line summaries, categories). |
| [log.md](log.md) | Append-only chronological ledger. |
| [done.md](done.md) | Completed planning tasks with links to evidence. |

## Directory map

- `meta/` — glossary, meta-planning process  
- `raw/` — immutable sources (links, excerpts); agents do not rewrite these as “truth updates”  
- `wiki/` — compiled knowledge: overview, requirements, architecture, decisions (ADRs), risks  
- `product/` — numbered product specs (lifecycle, mods, API, etc.)

## External pattern

The maintenance model follows the [LLM Wiki](https://gist.github.com/karpathy/442a6bf555914893e9891c11519de94f) idea: **raw sources → interlinked markdown → schema** (`WIKI_SCHEMA.md` + `.cursor/rules`).
