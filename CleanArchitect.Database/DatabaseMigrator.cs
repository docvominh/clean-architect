using System.Reflection;
using DbUp;
using DbUp.Engine;

namespace CleanArchitect.Database;

public static class DatabaseMigrator
{
    public static DatabaseUpgradeResult Migrate(string connectionString)
    {
        EnsureDatabase.For.SqlDatabase(connectionString);

        UpgradeEngine upgrader =
            DeployChanges.To
                .SqlDatabase(connectionString)
                .JournalToSqlTable("dbo", "_SchemaVersions")
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .WithTransactionPerScript()
                .LogToConsole()
                .Build();

        return upgrader.PerformUpgrade();
    }
}