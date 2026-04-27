# ASA dedicated process — runbook (MVP)

## Purpose

Normative **cwd**, **executable path**, **launch argument template**, and **stop** sequence so Phase C2 matches real Steam layouts.

## Paths

Let `B` = server `install_root` = `{ManagerDataRoot}\servers\{serverId}\binaries` (see [data-layout-windows.md](data-layout-windows.md) and [raw excerpt](../../raw/asa-install-layout-windows.md)).

| Item | Path |
|------|------|
| Executable | `B\ShooterGame\Binaries\Win64\ArkAscendedServer.exe` |
| Working directory for spawn | `B\ShooterGame\Binaries\Win64` |
| INI (canonical MVP edit target) | `B\ShooterGame\Saved\Config\WindowsServer\` |
| Saves | `B\ShooterGame\Saved\SavedArks\` |

## Launch template (replace placeholders from DB)

Spawn:

- **File**: `ArkAscendedServer.exe`
- **Cwd**: `...\ShooterGame\Binaries\Win64`
- **Args** (single string split per .NET rules — illustrative):

```text
{MapName}?listen?SessionName="{SessionName}"?Port={GamePort}?QueryPort={QueryPort}?RCONPort={RconPort}?ServerPassword=""?ServerAdminPassword="{RconPassword}"
```

- **`MapName`**: e.g. `TheIsland_WP` — must match ASA map token; validate against allowlist in impl.
- **Logging**: add `-log` / `-FullStdOutLogPath` style flags **after** spike on build; document chosen flags here when known.

## Stop sequence

1. If RCON configured: connect to `127.0.0.1:{RconPort}` (or bind address) and send **shutdown command** frozen in [rcon-mvp.md](../requirements/rcon-mvp.md) after test (`quit` / admin `DoExit` — **TBD exact string**, placeholder: `DoExit`).
2. Wait up to **T_shutdown** (default 60s) for process exit.
3. If still alive: **TerminateProcessTree** (Windows) and set state `Stopped` with warning flag.

## SteamCMD update

See [raw/asa-steamcmd-notes.md](../../raw/asa-steamcmd-notes.md) and [jobs-mvp.md](../requirements/jobs-mvp.md) for `2430930 validate` and preconditions.

## Agent acceptance checklist

- [ ] Start uses cwd + exe path above; failure if exe missing after successful install job.
- [ ] Stop always attempts RCON before kill when password set.
- [ ] INI read/write targets `WindowsServer` folder under `B`.

## Sources

- [raw/asa-install-layout-windows.md](../../raw/asa-install-layout-windows.md)
- [product/01-lifecycle.md](../../product/01-lifecycle.md)
