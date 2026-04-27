# Product: System foundations

Migrated from the former consolidated implementation outline. Open decisions: [open-questions.md](../wiki/open-questions.md).

## Notes

- This document is structured for direct use in AI coding tools (e.g. Cursor).
- Items below are modular and can be built incrementally; final acceptance criteria will be tightened after ADRs.

---

## 0. System foundations

### 0.1 Runtime / host layer

- Process management
  - Create abstraction layer for process execution
    - start(command, args, options)
    - stop(pid)
    - restart(pid)
  - Capture stdout/stderr streams
  - Emit lifecycle events
    - onStart
    - onExit
    - onError
  - Maintain process registry
    - serverId → PID mapping

- File system layer
  - **Windows contract (frozen):** see [data-layout-windows.md](../wiki/architecture/data-layout-windows.md) (`{ManagerDataRoot}\servers\{serverId}\...`). Legacy Unix-style `/data/servers/{serverId}/` paths in older notes are **non-normative**.
  - Define structure (logical)
    - `binaries/`
    - `configs/`
    - `mods/`
    - `logs/`
    - `backups/`
  - Implement utilities
    - ensureDir(path)
    - readFile/writeFile
    - copyDir/deleteDir
  - Log streaming
    - tail logs
    - expose via API/WebSocket

- Networking
  - Port allocation service
    - check availability
    - reserve ports
  - RCON client
    - connect(host, port, password)
    - send(command)
    - handle timeouts/retries

---

### 0.2 Core data models

**MVP persistence (normative):** [persistence-mvp.md](../wiki/architecture/persistence-mvp.md), [persistence-providers.md](../wiki/architecture/persistence-providers.md). **Post-MVP** models below marked.

- Server
  - id
  - name
  - map
  - installPath
  - ports {game, query, rcon}
  - status {stopped, starting, running, stopping, updating, crashed}
  - config (normalized JSON)

- **Cluster** _(post-MVP — not in MVP schema)_
  - id, name, serverIds[], sharedConfig, sharedMods

- **Mod** _(post-MVP registry — MVP manual paths only per mvp-scope)_
  - modId, name, version, enabled, installPath

- Job _(MVP subset: see persistence-mvp)_
  - id, type (`InstallOrUpdate` \| `Start` \| `Stop` \| `Restart` \| `Backup`), targetServerId, status, logs, timestamps

---

### 0.3 State management

- Persistent storage via EF Core: **SQLite default**, **PostgreSQL** switchable — [persistence-providers.md](../wiki/architecture/persistence-providers.md)
- In-memory runtime state
  - active processes
  - job queue
- Recovery on restart
  - reload DB
  - rebuild runtime state

---

## Related

- Next: [01-lifecycle.md](01-lifecycle.md)
- [Open questions](../wiki/open-questions.md)
- [ADR-0001: Host and deployment](../wiki/decisions/ADR-0001-host-and-deployment.md) (paths/OS)
