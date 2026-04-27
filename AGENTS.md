# AGENTS.md — instructions for AI agents (local or Cursor Cloud)

This repo is **spec-first**: almost all behavior is defined under [`plan/`](plan/). Your job is to turn that into **.NET code** without inventing product decisions in chat.

> **Name:** this file is `AGENTS.md` (Cursor / tools look for this path). If you were pointed at a typo filename, use this file.

---

## 1. Bootstrap (read in order)

1. [`plan/README.md`](plan/README.md) — map + reading order  
2. [`plan/wiki/implementation-backbone.md`](plan/wiki/implementation-backbone.md) — **canonical coding order** (C0 → C4)  
3. [`plan/wiki/requirements/mvp-scope.md`](plan/wiki/requirements/mvp-scope.md) — what MVP is / is not  
4. [`plan/wiki/requirements/api-v1-mvp.md`](plan/wiki/requirements/api-v1-mvp.md) — REST contract  
5. [`plan/wiki/architecture/persistence-providers.md`](plan/wiki/architecture/persistence-providers.md) — SQLite default, Postgres optional  
6. [`plan/wiki/architecture/asa-process-runbook.md`](plan/wiki/architecture/asa-process-runbook.md) — process paths (Windows)  
7. [`plan/wiki/risks/asa-known-issues.md`](plan/wiki/risks/asa-known-issues.md) — pitfalls to design around  
8. [`plan/WIKI_SCHEMA.md`](plan/WIKI_SCHEMA.md) — if you touch `plan/**` docs  
9. [`plan/wiki/meta/agent-execution-conventions.md`](plan/wiki/meta/agent-execution-conventions.md) — shape of normative pages  

Pre-code **gate** evidence: [`plan/meta/how-we-plan.md`](plan/meta/how-we-plan.md). Implementation may proceed when a human agrees the snapshot still matches reality.

---

## 2. Cloud agent scope (“initial heavy lifting”)

**In scope** for a cloud/sandbox agent without expecting full end-to-end ASA:

- **Phase C0**: .NET 8 solution, Worker Service + ASP.NET Core host, `/health`, config + logging layout, project structure.  
- **Phase C1**: EF Core `AppDbContext`, migrations, repositories, `servers` + `jobs` per [`persistence-mvp.md`](plan/wiki/architecture/persistence-mvp.md); unit tests with **SQLite in-memory** or file DB.  
- **Phase C3 (partial)**: Minimal APIs behind **`X-Api-Key`** matching [`api-v1-mvp.md`](plan/wiki/requirements/api-v1-mvp.md); **mock** SteamCMD and game process I/O where needed; OpenAPI artifact as required there.  
- **CI**: `dotnet build` / `dotnet test` on **Linux** runner is fine for this layer.

**Explicitly out of scope / do not require in cloud**

- Installing or running **real SteamCMD** or **ArkAscendedServer.exe** (Windows-only, large, interactive).  
- Proving **Windows Service** registration end-to-end (document steps; verify on a Windows host).  
- Storing or logging **real secrets** (RCON, API keys, Steam passwords).

**Defer with clear `// TODO(WIN-SMOKE):`** comments**

- Service install (`sc.exe` / `New-Service`), real Kestrel bind to non-loopback, real firewall behavior.  
- Full ASA launch line tuning after a human captures one successful run on hardware.

---

## 3. Rules

- **Do not** change product meaning without updating **`plan/`** (then `index.md` / `log.md` / `done.md` per [`WIKI_SCHEMA.md`](plan/WIKI_SCHEMA.md)).  
- **Secrets:** env vars or local JSON outside git only; never commit credentials.  
- **Persistence:** no raw SQL in Minimal API delegates — use injected services / `DbContext`; support **Sqlite | Postgres** per [`persistence-providers.md`](plan/wiki/architecture/persistence-providers.md).  
- **Jobs:** respect concurrency rules in [`jobs-mvp.md`](plan/wiki/requirements/jobs-mvp.md) (e.g. no install while server running).  
- **Cursor:** when editing only `plan/**`, follow [`.cursor/rules/ark-planning.mdc`](.cursor/rules/ark-planning.mdc).

---

## 4. Suggested first task for Cloud Agent

> Implement **Phase C0 + C1** from [`implementation-backbone.md`](plan/wiki/implementation-backbone.md): solution skeleton, configuration, logging, EF models + migrations for MVP tables, repository tests. Stub SteamCMD and process supervisor interfaces with interfaces + no-op or fake implementations. Open a PR with `dotnet test` green on Linux CI.

---

## 5. After cloud: human / Windows follow-up

On a **Windows Server** (or dev VM): run SteamCMD smoke, service install, real start/stop, then continue **C2** per the backbone doc and [`plan/product/12-deployment.md`](plan/product/12-deployment.md).
