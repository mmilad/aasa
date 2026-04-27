# Glossary

| Term | Meaning (in this project) |
|------|---------------------------|
| **ASA** | ARK: Survival Ascended — dedicated server and client ecosystem targeted by this manager. |
| **SteamCMD** | Valve’s command-line Steam client used to install/update dedicated server builds. |
| **Server** | One managed dedicated server instance: binaries, configs, ports, lifecycle. |
| **Cluster** | A logical group of servers sharing travel/cluster configuration and optionally mods. |
| **RCON** | Remote console protocol for admin commands and graceful shutdown attempts. |
| **Job** | A queued or scheduled unit of work (update, restart, backup). |
| **Mod** | CurseForge (or compatible) content installed into a server’s mod path. |
| **Plugin (extensibility)** | External, self-hosted extension integrated via a documented contract (HTTP/gRPC, manifests)—not the same as an ASA game mod unless stated. |
| **Schema-driven UI** | User-facing forms generated from machine-readable setting schemas rather than bespoke screens per field. |
| **ADR** | Architecture Decision Record under `plan/wiki/decisions/`. |
| **Ambiguity gate** | Checklist in [how-we-plan.md](how-we-plan.md) that must pass before implementation work begins. |
| **.NET / C#** | Chosen implementation stack: **.NET 8 (LTS)** Worker Service + ASP.NET Core on Windows Server ([tech-stack.md](../wiki/architecture/tech-stack.md)). |
| **MVP v0.1** | First shippable slice defined in [mvp-scope.md](../wiki/requirements/mvp-scope.md). |
| **Implementation backbone** | Ordered coding phases in [implementation-backbone.md](../wiki/implementation-backbone.md). |
