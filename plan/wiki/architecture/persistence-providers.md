# Persistence providers (SQLite and PostgreSQL)

## Purpose

All database access goes through a **single abstraction** so the product can switch **`Sqlite` | `Postgres`** via configuration without rewriting handlers. **No ADO.NET/SQL in controllers** — use injected services / `DbContext`.

## Normative choice (MVP)

- **ORM**: EF Core **8** (aligns with [tech-stack.md](tech-stack.md)).
- **Registration**: `AddDbContext<AppDbContext>((sp, opt) => { ... })` reads `Database:Provider` and calls `UseSqlite(...)` or `UseNpgsql(...)`.
- **Exactly one** provider is active per process lifetime.

## Configuration contract

```json
"Database": {
  "Provider": "Sqlite",
  "Sqlite": {
    "DataSource": "%ManagerDataRoot%/manager.db"
  },
  "Postgres": {
    "ConnectionString": "Host=...;Database=...;Username=...;Password=..."
  }
}
```

- **`Provider`**: `Sqlite` (default) | `Postgres` (case-insensitive).
- **Sqlite `DataSource`**: absolute or relative path; **recommended** under `ManagerDataRoot` (see [data-layout-windows.md](data-layout-windows.md)); file name e.g. `manager.db`.
- **Postgres**: standard Npgsql connection string; secrets belong in ACL-protected config, not repo.

## Migrations

- Single EF model; generate migrations that work for **both** providers where possible.
- **Sharp edges** (document in implementation notes): type width, `bool`, timestamps with time zone, sequences — use provider-agnostic column types; test `dotnet ef database update` on **both** in CI when Postgres job exists.

## Agent acceptance checklist

- [ ] Swapping only `Database:Provider` + valid connection string boots API (integration test or manual checklist).
- [ ] `dotnet ef migrations add` run against Sqlite dev DB succeeds.
- [ ] Documented “Postgres first run” applies migrations before accepting traffic.
- [ ] No handler references `SqliteConnection` or `NpgsqlConnection` directly.

## Sources

- [persistence-mvp.md](persistence-mvp.md)
- [ADR-0001](../decisions/ADR-0001-host-and-deployment.md)
