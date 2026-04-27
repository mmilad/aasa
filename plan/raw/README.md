# Raw sources (immutable layer)

## Purpose

Material placed here is **source input** for planning: official doc links, pasted excerpts, screenshots described in text, notes from experiments. Agents **read** this layer; they do not rewrite it to “fix” contradictions. Supersession happens in `plan/wiki/` (with ADRs) or in `plan/product/` with explicit pointers.

## Legacy snapshot

- [legacy-ark-server-manager-plan-monolith.md](legacy-ark-server-manager-plan-monolith.md) — frozen copy of the former single-file implementation outline before migration into `plan/product/`.

## What belongs here

- URLs and bibliographic keys (see [sources.bib.md](sources.bib.md))
- Short verbatim excerpts with attribution and date retrieved
- Redacted logs or config snippets (no secrets)

## What does not belong here

- Secrets: API keys, Steam passwords, RCON passwords, private IPs if policy forbids
- Large proprietary binaries (link externally or store outside git with policy in team docs)

## Git and size

- Prefer links + small excerpts in-repo.
- Optional: add patterns to `.gitignore` for large local dumps under `raw/_local/` if your team uses that pattern (not committed by default).
