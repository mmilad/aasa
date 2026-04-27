# ASA dedicated — install layout on Windows (immutable planning excerpt)

> Secondary sources: community guides + wiki; **re-verify** on a real `app_update 2430930` tree before locking launch args. Add dated sibling file instead of rewriting facts here.

## Root

SteamCMD `+force_install_dir` should point at the server’s **`binaries`** directory from [data layout](../wiki/architecture/data-layout-windows.md), e.g. `{ManagerDataRoot}\servers\{serverId}\binaries\`.

## Expected relative paths (under `force_install_dir`)

| Relative path | Role |
|----------------|------|
| `ShooterGame\Binaries\Win64\ArkAscendedServer.exe` | Dedicated server executable |
| `ShooterGame\Saved\Config\WindowsServer\Game.ini` | Server config |
| `ShooterGame\Saved\Config\WindowsServer\GameUserSettings.ini` | Server settings |
| `ShooterGame\Saved\SavedArks\` | World / save data |

## Launch pattern (illustrative)

From `ShooterGame\Binaries\Win64\`, run `ArkAscendedServer.exe` with map, `listen`, `SessionName`, `Port`, `QueryPort`, etc. Exact query string is specified in [asa-process-runbook.md](../wiki/architecture/asa-process-runbook.md).

## Sources (URLs)

- https://steamcommunity.com/sharedfiles/filedetails/?id=3382155003  
- https://ark.wiki.gg/wiki/Dedicated_server_setup  
- https://steamdb.info/app/2430930/
