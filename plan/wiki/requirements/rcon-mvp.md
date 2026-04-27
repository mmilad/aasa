# RCON — MVP contract

## Purpose

Graceful shutdown before kill; avoid treating **single RCON timeout** as fatal crash (community-reported flakiness).

## Protocol

- **Source RCON** (Valve / Source query family). Pin link in [sources.bib.md](../../raw/sources.bib.md) as `rcon-protocol` (implementation adds URL).

## MVP commands

1. **Shutdown**: send server-admin shutdown string documented in [asa-process-runbook.md](../architecture/asa-process-runbook.md) (`DoExit` / `Quit` / game-specific — **freeze exact string** in runbook after one successful capture test).
2. **List players**: **deferred to phase 1.1** unless trivial single-line parse is proven in spike — MVP may omit API exposure for listplayers.

## Timeouts and retries

- Connect timeout: **5s** (illustrative; fix in appsettings).
- Command wait for shutdown: **60s** grace (configurable `T_shutdown`).
- On TCP failure: **retry up to 2** times with exponential backoff (1s, 3s); **do not** kill process on RCON failure alone — proceed to kill only if still alive after grace **or** operator called `Stop` with `force=true` (post-MVP optional flag).

## Logging

- Never log RCON password or full packet payloads at `Information`.

## Agent acceptance checklist

- [ ] Stop path: RCON attempt logged (without secrets) → wait → process exit or kill.
- [ ] Single RCON error does not flip server to `Crashed` if OS process still running.

## Sources

- [ADR-0003](../decisions/ADR-0003-secrets-and-exposure.md)
- [product/04-players-rcon.md](../../product/04-players-rcon.md)
