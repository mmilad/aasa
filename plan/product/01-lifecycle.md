# Product: Server lifecycle

Migrated from the former consolidated implementation outline.

---

## 1. Server lifecycle

### 1.1 Create server

- Validate input
- Generate ID
- Create directories
- Generate default config
- Persist server

---

### 1.2 Install server (SteamCMD)

- Ensure SteamCMD installed
- Execute install command
- Stream logs
- Validate binaries

**MVP pinned identifiers** (do not implement without re-reading raw):

- Dedicated server Steam app id: **`2430930`** (not the ASA client `2399830`). Source: [raw/asa-steamcmd-notes.md](../raw/asa-steamcmd-notes.md) and [sources.bib.md](../raw/sources.bib.md).
- Example command shape and platform notes: same raw excerpt.

---

### 1.3 Start / stop / restart

- Start
  - build command
  - spawn process
  - store PID
- Stop
  - try RCON shutdown
  - fallback kill
- Restart
  - stop → start

---

### 1.4 Crash detection

- Watchdog loop
- Detect process exit
- Update status
- Optional auto-restart with backoff

---

## Related

- Previous: [00-foundations.md](00-foundations.md)
- Next: [02-updates-automation.md](02-updates-automation.md)
- [asa-process-runbook.md](../wiki/architecture/asa-process-runbook.md)
- [asa-known-issues.md](../wiki/risks/asa-known-issues.md)
- [jobs-mvp.md](../wiki/requirements/jobs-mvp.md)
- [raw/sources.bib.md](../raw/sources.bib.md)
