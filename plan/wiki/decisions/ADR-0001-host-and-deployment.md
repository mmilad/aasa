# ADR-0001: Host OS, deployment shape, and implementation language

- **Status**: accepted  
- **Date**: 2026-04-27  
- **Supersedes**: prior “Linux-first” framing in early stubs  

## Context

The manager will run on **Windows Server**. ARK: Survival Ascended dedicated server is treated as **Windows-native** for this product (Linux hosting for ASA is out of scope for MVP — see [raw/asa-steamcmd-notes.md](../../raw/asa-steamcmd-notes.md) and pinned sources).

The implementer is **not** assumed to know Go or .NET deeply; choosing a single mainstream Windows stack reduces incidental complexity.

Tracks (resolved): ~~[open-questions.md](../open-questions.md) OQ-001~~.

## Decision

1. **Manager host OS**: **Windows Server** only for MVP and the initial architecture baseline.  
2. **Game server**: **ASA dedicated Windows build** installed via **SteamCMD** to per-server `binaries\` under the manager data root (see [data layout](../architecture/data-layout-windows.md)).  
3. **Implementation language**: **C# on .NET 8 (LTS)** hosted as a **Windows Service** using **Worker Service + ASP.NET Core** (see [tech-stack.md](../architecture/tech-stack.md)).  
4. **Out of scope (MVP)**: Linux-hosted manager; Linux-native ASA server assumptions; Docker-first deployment (may be revisited later without changing MVP code paths).

## Consequences

- Positive: Native Windows service integration, predictable paths, aligns with SteamCMD examples for Windows, large tutorial base for .NET services.  
- Negative: Cross-platform manager is not free; all path and process abstractions may assume Windows until an ADR explicitly ports.

## Alternatives considered

- **Go** for single-binary deployment — rejected for MVP because Windows Service + beginner ergonomics favor .NET given operator environment.  
- **Dual-target manager (Windows + Linux)** — rejected for MVP to remove branching until product is proven on Windows Server.  
- **Node/TypeScript** — acceptable technically; rejected to keep one cohesive Microsoft stack on Windows Server for services + docs.

## Sources

- [tech-stack.md](../architecture/tech-stack.md)
- [data-layout-windows.md](../architecture/data-layout-windows.md)
- [sources.bib.md](../../raw/sources.bib.md)
