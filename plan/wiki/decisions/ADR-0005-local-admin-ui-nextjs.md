# ADR-0005: Local admin UI — Next.js (single-language .NET control plane)

- **Status**: accepted
- **Date**: 2026-05-11

## Context

The manager is an **API-first** control plane ([api-v1-mvp.md](../requirements/api-v1-mvp.md)). The Worker REST API requires a shared secret presented as HTTP header `X-Api-Key` ([ADR-0003](ADR-0003-secrets-and-exposure.md)).

The product team wants an operator admin UI that:

- Can run as a **separate web app** and talk to the Worker API over HTTP.
- Avoids exposing the Worker API key to the browser.
- Initially supports a **POC** where humans do not have a full login system (no user auth), but still requires Worker API authentication.

An earlier decision (ADR-0004) selected a C# Blazor UI for the local operator panel. This ADR updates the normative UI direction for the operator POC.

## Decision

1. **Normative UI stack**: a **Next.js** admin UI (App Router) as the local operator panel.
2. **Security model**: the browser calls the Next.js server endpoints; the Next.js server calls the Worker API and injects `X-Api-Key` **server-side**.
   - The Worker API key is loaded from server-side configuration (env/config/user secrets).
   - The key is never embedded in browser-delivered JavaScript bundles (no `NEXT_PUBLIC_` usage).
3. **Loopback-first**: default bind the UI to loopback only (e.g. `http://127.0.0.1:<port>`). Any external/public exposure is opt-in and must warn at startup, consistent with [ADR-0003](ADR-0003-secrets-and-exposure.md).
4. **MVP UI surface** (POC): list servers, create servers, enqueue at least one job action, start/stop, and poll job status.

## Consequences

- Positive: fast operator UI iteration for the web; avoids secret exposure by keeping Worker calls server-side.
- Positive: the Worker API remains the single source of truth for lifecycle, jobs, and logs.
- Negative: introduces a separate runtime (Node) and a separate build pipeline for the UI.
- Negative: additional work for strongly typed client models unless using code generation from OpenAPI.

## Alternatives considered

- Keep Blazor as normative UI (ADR-0004) — rejected as the product team’s requested operator UI direction for the POC.
- Next.js client-side calling Worker directly — rejected because it would require exposing `X-Api-Key` to the browser.
- Desktop UI (WPF/WinUI/Avalonia) — deferred; web UI iteration is prioritized for this POC.

## Sources

- [api-v1-mvp.md](../requirements/api-v1-mvp.md)
- [ADR-0003](ADR-0003-secrets-and-exposure.md)
- [mvp-scope.md](../requirements/mvp-scope.md)

