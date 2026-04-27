# ASA dedicated server — SteamCMD notes (immutable excerpt)

> Planning snapshot — verify against Steam/Valve before production cutover; update by **adding** a new dated excerpt rather than editing facts here.

## App IDs (do not confuse)

| App | Steam AppID | Role |
|-----|-------------|------|
| ARK: Survival Ascended **Dedicated Server** | **2430930** | Use with `steamcmd ... +app_update 2430930` for dedicated server files |
| ARK: Survival Ascended **client** | 2399830 | Not the dedicated server depot for headless hosting |

**MVP assumption:** SteamCMD uses **`2430930`** for installing/updating the dedicated server.

## Typical anonymous login flow (Windows)

Documented in community guides; exact flags pinned at implementation:

```text
steamcmd.exe +force_install_dir "<ServerBinariesPath>" +login anonymous +app_update 2430930 validate +quit
```

## Platform note (planning assumption)

Community and third-party databases describe the dedicated server app as **Windows-oriented**; Linux hosting typically implies compatibility layers. Manager planning assumes **Windows native** dedicated server alongside the manager on Windows Server.

## Sources (URLs)

- SteamDB app metadata: `https://steamdb.info/app/2430930/`  
- Community wiki (verify current text): `https://ark.wiki.gg/wiki/Dedicated_server_setup`  
- Also indexed in [sources.bib.md](sources.bib.md).
