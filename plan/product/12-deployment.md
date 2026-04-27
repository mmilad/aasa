# Product: Deployment

Migrated from the former consolidated implementation outline.

---

## 12. Deployment

- Local setup
- Run as service
- Docker support (optional)

### MVP deployment shape (Windows Server)

- Ship as **Windows Service** hosting the .NET worker + Kestrel per [ADR-0001](../wiki/decisions/ADR-0001-host-and-deployment.md) and [tech-stack.md](../wiki/architecture/tech-stack.md).
- Installation doc must include: service account choice, `ManagerDataRoot` ACLs, firewall note (loopback default), where to place `apikey.json` (name illustrative).
- **Docker**: non-goal for MVP per [mvp-scope.md](../wiki/requirements/mvp-scope.md); revisit only with new ADR.

### MVP operator checklist (Windows Server)

1. **OS**: Windows Server 2019+ (64-bit); install **latest VC++ redistributables** (x64) — prerequisite for native deps.
2. **Service account**: default **Virtual Service Account** `NT SERVICE\ArkServerManager` (illustrative name) **or** `LocalSystem` for lab only — document least-privilege alternative when using dedicated domain account.
3. **Create data root**: `%ProgramData%\ArkServerManager\` (or override `ARKMGR_DATA_ROOT`). Grant service account **Modify** on tree.
4. **Secrets**: place `appsettings.Production.json` + `secrets\apikey.json` (illustrative) with **Administrators + service account** read only.
5. **Publish binaries**: copy build output to `C:\Program Files\ArkServerManager\` (illustrative); add folder to AV **exclusions** if real-time scan locks SteamCMD files.
6. **Register service** (pick one canonical path in impl docs):
   - `sc.exe create ArkServerManager binPath= "\"C:\\...\\ArkServerManager.exe\"\" --windows-service\"" start= auto`
   - or `New-Service` PowerShell equivalent for .NET Worker template.
7. **Firewall**: loopback-only API needs **no** inbound rule; if `Listen:Public`, open chosen **TCP** port for Kestrel.
8. **Game visibility**: open **UDP+TCP** for `game_port` / `query_port` on public interfaces when hosting for internet clients.
9. **SteamCMD**: install `steamcmd.exe` to e.g. `C:\SteamCMD\`; service config points `SteamCmdPath`.
10. **Troubleshooting**: see [asa-known-issues.md](../wiki/risks/asa-known-issues.md) (SteamCMD validate, clean reinstall, stuck updates).

### Manual restore (when backups used)

1. `Stop` server via API.  
2. Unzip backup so `ShooterGame\Saved\...` and `WindowsServer\*.ini` paths align with [asa-process-runbook.md](../wiki/architecture/asa-process-runbook.md).  
3. `Start` server; verify world loads.

---

## Related

- Previous: [11-security.md](11-security.md)
- Next: [13-advanced.md](13-advanced.md)
- [ADR-0001: Host and deployment](../wiki/decisions/ADR-0001-host-and-deployment.md)
- [implementation-backbone.md](../wiki/implementation-backbone.md) (first code milestones)
- [production-readiness.md](../wiki/requirements/production-readiness.md)
