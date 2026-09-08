namespace CleanArchitect.Infrastructure.IntegrationTests.Fixtures;

public abstract class RepositoryTestBase(MsSqlContainerFixture fixture) : IAsyncLifetime
{
    protected AppDbContext Context { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await fixture.ResetAsync();
        Context = CreateDbContext();
    }

    public async Task DisposeAsync()
    {
        await Context.DisposeAsync();
    }

    protected AppDbContext CreateDbContext()
    {
        return fixture.CreateDbContext();
    }

    protected async Task<Guid> CreateIdentityUserAsync()
    {
        var id = Guid.NewGuid();
        Context.Users.Add(new AppUser { Id = id, UserName = $"{id}@example.com", Email = $"{id}@example.com" });
        await Context.SaveChangesAsync();
        return id;
    }
}
