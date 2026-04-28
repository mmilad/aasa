namespace ArkServerManager.Worker.Configuration;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";
    public string Provider { get; set; } = "Sqlite";
    public SqliteOptions Sqlite { get; set; } = new();
    public PostgresOptions Postgres { get; set; } = new();
}

public sealed class SqliteOptions
{
    public string DataSource { get; set; } = "data/manager.db";
}

public sealed class PostgresOptions
{
    public string ConnectionString { get; set; } = string.Empty;
}
