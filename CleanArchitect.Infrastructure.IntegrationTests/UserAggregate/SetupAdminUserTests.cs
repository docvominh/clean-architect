using CleanArchitect.Api;
using CleanArchitect.Application.UserAggregate;
using CleanArchitect.Infrastructure.IntegrationTests.Fixtures;
using CleanArchitect.Infrastructure.UserAggregate;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Shouldly;

namespace CleanArchitect.Infrastructure.IntegrationTests.UserAggregate;

[Collection("MsSql")]
public sealed class SetupAdminUserTests(MsSqlContainerFixture fixture) : RepositoryTestBase(fixture)
{
    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData(" ", " ")]
    public async Task WithoutCredentials_CreatesRolesOnly(string? email, string? password)
    {
        // Arrange
        await using var app = CreateApp(email, password);

        // Act
        await app.SetupAdminUserAsync();

        // Assert
        (await Context.Users.CountAsync()).ShouldBe(0);
        (await Context.UserProfiles.CountAsync()).ShouldBe(0);
        (await Context.Roles.Select(role => role.Name).ToListAsync()).ShouldBe(["Admin", "User"], ignoreOrder: true);
    }

    [Fact]
    public async Task DevelopmentWithConfiguredDemoCredentials_CreatesAdmin()
    {
        // Arrange
        await using var app = CreateApp("demo@example.com", "demo");

        // Act
        await app.SetupAdminUserAsync();

        // Assert
        await AssertAdminAsync(app, "demo@example.com", "demo");
    }

    [Fact]
    public async Task ConfiguredCredentials_CreatesAdminAndIsIdempotent()
    {
        // Arrange
        await using var app = CreateApp("owner@example.com", "Strong-Test-123!");

        // Act
        await app.SetupAdminUserAsync();
        await app.SetupAdminUserAsync();

        // Assert
        await AssertAdminAsync(app, "owner@example.com", "Strong-Test-123!");
        (await Context.Users.CountAsync()).ShouldBe(1);
        (await Context.UserProfiles.CountAsync()).ShouldBe(1);
    }

    [Theory]
    [InlineData("owner@example.com", null)]
    [InlineData(null, "Strong-Test-123!")]
    [InlineData("owner@example.com", " ")]
    [InlineData(" ", "Strong-Test-123!")]
    public async Task IncompleteCredentials_RejectsBootstrap(string? email, string? password)
    {
        // Arrange
        await using var app = CreateApp(email, password);

        // Act
        Func<Task> act = () => app.SetupAdminUserAsync();

        // Assert
        var error = await Should.ThrowAsync<InvalidOperationException>(act);
        error.Message.ShouldBe("BootstrapAdmin:Email and BootstrapAdmin:Password must both be configured.");
        (await Context.Users.CountAsync()).ShouldBe(0);
        (await Context.Roles.CountAsync()).ShouldBe(0);
    }

    [Fact]
    public async Task InvalidEmail_RejectsBootstrap()
    {
        // Arrange
        await using var app = CreateApp("invalid user name", "demo");

        // Act
        Func<Task> act = () => app.SetupAdminUserAsync();

        // Assert
        var error = await Should.ThrowAsync<InvalidOperationException>(act);
        error.Message.ShouldContain("InvalidUserName");
        (await Context.Users.CountAsync()).ShouldBe(0);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ExistingNonAdmin_IsPromotedWithoutChangingPassword(bool matchingPassword)
    {
        // Arrange
        const string email = "existing@example.com";
        const string originalPassword = "Original-Test-123!";
        await using var app = CreateApp(email, matchingPassword ? originalPassword : "Different-Test-456!");
        await using var scope = app.Services.CreateAsyncScope();
        var manager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var user = new AppUser { UserName = email, Email = email };
        (await manager.CreateAsync(user, originalPassword)).Succeeded.ShouldBeTrue();

        // Act
        await app.SetupAdminUserAsync();

        // Assert
        (await manager.IsInRoleAsync(user, "Admin")).ShouldBeTrue();
        (await manager.CheckPasswordAsync(user, originalPassword)).ShouldBeTrue();
        (await Context.UserProfiles.AnyAsync(profile => profile.Id == user.Id)).ShouldBeTrue();
    }

    private WebApplication CreateApp(string? email = null, string? password = null)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Development" });
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["BootstrapAdmin:Email"] = email,
            ["BootstrapAdmin:Password"] = password
        });
        builder.Services.AddScoped(_ => CreateDbContext());
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddIdentityCore<AppUser>(options => options.Password.RequiredLength = 8)
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>();
        return builder.Build();
    }

    private async Task AssertAdminAsync(WebApplication app, string email, string password)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var manager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var user = await manager.FindByEmailAsync(email);
        user.ShouldNotBeNull();
        (await manager.CheckPasswordAsync(user, password)).ShouldBeTrue();
        (await manager.IsInRoleAsync(user, "Admin")).ShouldBeTrue();
        var profile = await Context.UserProfiles.SingleAsync(profile => profile.Id == user.Id);
        profile.DisplayName.ShouldBe("Admin");
    }
}
