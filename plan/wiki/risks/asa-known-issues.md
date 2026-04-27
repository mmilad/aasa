# Known ASA / SteamCMD pitfalls and mitigations

## Purpose

Record **recurring admin-community failure modes** so the manager and runbooks avoid them. Confidence: **secondary** (forums/Steam) — verify on real hosts; cite in [sources.bib.md](../../raw/sources.bib.md).

## Symptom matrix

| Symptom | Likely cause | Manager mitigation | Operator / runbook | Test idea |
|---------|--------------|--------------------|--------------------|-----------|
| Stuck old build after `app_update` | missing `validate`; partial download | Always `app_update 2430930 validate`; capture exit code | Clean reinstall: delete under `binaries` except retain `Saved` policy per doc | Job log shows `Success` + build id TBD |
| SteamCMD itself broken | stale self-update | Detect non-zero + log “re-download SteamCMD” | [12-deployment](../../product/12-deployment.md) troubleshooting | Manual |
| Update corrupts files | AV / locks | **Block** install job unless server `Stopped` ([jobs-mvp](../requirements/jobs-mvp.md)) | Exclusion for `binaries` path | `409` when running |
| Server not in browser list | firewall / wrong query port | Expose configured ports in `GET .../status` | Open UDP/TCP for game + query | External client |
| Exe flashes / Event 1000 | VC++ / DirectX | Preflight doc | Install VC++ redists | Run exe `--help` smoke |
| Crash on boot after mod | bad mod | MVP: manual mod paths; doc “remove mods” | Disable mods in INI / folder rename | Start with clean `mods` |
| Restart loop | corrupt save | Graceful stop; backups include `SavedArks` | Restore previous save | Kill mid-write should be impossible (stop first) |
| RCON flaky | TCP drops | Backoff + don’t mark crashed on one timeout ([rcon-mvp](../requirements/rcon-mvp.md)) | — | Chaos test disconnect |
| CurseForge outage | external API | MVP no auto CurseForge | Retry later | — |

## Pseudocode (planning — align with impl)

```text
RunSteamCmdUpdate(serverId, installDir, steamCmdExe) -> JobResult:
  require ServerState(serverId) == Stopped else return 409 SERVER_NOT_STOPPED
  args = force_install_dir(installDir), login anonymous, app_update 2430930 validate, quit
  run with streamed logs; non-zero exit -> Failed with hint CLEAN_REINSTALL
```

```text
StopServerGracefully(serverId):
  if rconConfigured: send DoExit_or_frozen_string; wait T_shutdown
  if processAlive: kill tree; log warn
```

## Sources

- https://steamcommunity.com/app/2399830/discussions/0/3941272762722295345/ (SteamCMD update thread)  
- https://www.reddit.com/r/ArkSurvivalAscended/comments/1hmy8he/dedicated_server_not_updating_stuck_on_5615/  
- https://survivetheark.com/index.php?/forums/topic/662900-dedicated-service-using-steamcmd-not-getting-updates-in-timely-fashion/  
- [asa-steamcmd-notes.md](../../raw/asa-steamcmd-notes.md)
