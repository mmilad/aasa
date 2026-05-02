# ADR-0004: Local admin UI — Blazor (.NET single language)

- **Status**: accepted  
- **Date**: 2026-05-02  

## Context

The control plane is **API-first** ([api-v1-mvp.md](../requirements/api-v1-mvp.md)). Earlier [mvp-scope.md](../requirements/mvp-scope.md) listed a **public** browser dashboard as a non-goal. The product team wants a **single-language** (.NET) operator experience, **not** a TypeScript SPA, while keeping the **same exposure discipline** as [ADR-0003](ADR-0003-secrets-and-exposure.md) (loopback-first, shared `X-Api-Key`).

## Decision

1. **Normative UI stack**: **Blazor Web App** with **Interactive Server** render mode for the **local operator admin panel** (C# end-to-end with the Worker/API).  
2. **Hosting**: **Separate ASP.NET Core project** in the same solution (e.g. `ArkServerManager.Admin`), **not** a public SaaS-style deployment. Default bind **loopback only** (e.g. `http://127.0.0.1:<port>`), with any LAN/public bind **opt-in** and **warned** the same way as the Worker `Listen:Public` story.  
3. **Integration**: Admin app talks to the Worker via **`HttpClient`** against **`/api/v1/...`**, sending **`X-Api-Key`** using the **same secret configuration patterns** as ADR-0003 (no API key embedded in client-side WASM bundles; Interactive Server keeps secrets server-side).  
4. **Explicit non-choice**: **No** normative **TypeScript/React/Vue** SPA for this product’s first-party UI (third-party consumers may still use the OpenAPI contract however they want).

## Consequences

- **Positive**: One language for features spanning API + UI; fast iteration for server list, jobs, logs, INI forms; aligns with Windows operator workflows (RDP → browser on localhost).  
- **Negative**: Second process (or merged-host later), SignalR circuit lifecycle for Blazor Server, and **CORS is irrelevant** if both apps are loopback and server-side calls originate from the Admin host.

## Alternatives considered

- **WPF / WinUI desktop** — rejected as **normative** first UI: excellent for a thick client, but slower for data-dense admin (logs, tables) and duplicates HTTP contract work; may revisit for a packaged “.exe console” later.  
- **Blazor WebAssembly calling API from browser** — rejected as **primary**: API key handling in the browser is weaker unless a BFF is added; Interactive Server avoids that for MVP.  
- **TypeScript SPA** — rejected per product preference and single-language goal.

## Sources

- [mvp-scope.md](../requirements/mvp-scope.md)  
- [ADR-0003](ADR-0003-secrets-and-exposure.md)  
- [implementation-backbone.md](../implementation-backbone.md)  
