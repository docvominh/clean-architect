using CleanArchitect.Database;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using Testcontainers.MsSql;

namespace CleanArchitect.Infrastructure.IntegrationTests.Fixtures;

public sealed class MsSqlContainerFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container =
        new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04").Build();

    private string ConnectionString => new SqlConnectionStringBuilder(_container.GetConnectionString())
    {
        InitialCatalog = "clean_architect_tests"
    }.ConnectionString;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        var result = DatabaseMigrator.Migrate(ConnectionString);
        if (!result.Successful) throw new InvalidOperationException("Failed to migrate the SQL Server test database.", result.Error);
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    public AppDbContext CreateDbContext()
    {
        return new AppDbContext(
            new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(ConnectionString)
                .Options);
    }

    public async Task ResetAsync()
    {
        await using var context = CreateDbContext();

        // Delete dependents first; retain the migration journal and schema.
        await context.Database.ExecuteSqlRawAsync(
            """
            DELETE FROM [OrderProducts];
            DELETE FROM [Orders];
            DELETE FROM [Products];
            DELETE FROM [UserAddresses];
            DELETE FROM [Users];
            DELETE FROM [RefreshTokens];
            DELETE FROM [AspNetUsers];
            DELETE FROM [AspNetRoles];
            """);
    }
}
