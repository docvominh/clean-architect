using CleanArchitect.Domain.UserAggregate;
using CleanArchitect.Infrastructure.IntegrationTests.Fixtures;
using CleanArchitect.Infrastructure.UserAggregate;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using Shouldly;

namespace CleanArchitect.Infrastructure.IntegrationTests.UserAggregate;

[Collection("MsSql")]
public sealed class UserRepositoryTests(MsSqlContainerFixture fixture) : RepositoryTestBase(fixture)
{
    [Fact]
    public async Task Add_PersistsProfileAndAddressesWhenUnitOfWorkSaves()
    {
        // Arrange
        var id = await CreateIdentityUserAsync();
        var user = new User(id, id, "Test user");
        user.UpdateAddress(
        [
            new UserAddress(Guid.NewGuid(), id, id, "Vietnam", "HCMC", "1 Main Street", "0123456789", isDefault: true),
            new UserAddress(Guid.NewGuid(), id, id, "Vietnam", "Hanoi", "2 Main Street", "0987654321", "Hanoi")
        ]);

        // Act
        await new UserRepository(Context).Add(user);
        bool wasMissingBeforeSave;
        await using (var beforeSave = CreateDbContext())
        {
            wasMissingBeforeSave = await new UserRepository(beforeSave).FindAsync(id, CancellationToken.None) is null;
        }

        await Context.SaveChangesAsync();

        // Assert
        wasMissingBeforeSave.ShouldBeTrue();
        await using var verification = CreateDbContext();
        var saved = await new UserRepository(verification).FindAsync(id, CancellationToken.None);
        saved.ShouldNotBeNull();
        saved.DisplayName.ShouldBe("Test user");
        saved.CreateBy.ShouldBe(id);
        saved.CreatedAt.ShouldNotBe(default);
        var addresses = await verification.UserAddresses.Where(a => a.UserId == id).ToListAsync();
        addresses.Count.ShouldBe(2);
        var primary = addresses.Where(a => a.IsDefault).ShouldHaveSingleItem();
        primary.Country.ShouldBe("Vietnam");
        primary.City.ShouldBe("HCMC");
        primary.Street.ShouldBe("1 Main Street");
        primary.ContactPhoneNumber.ShouldBe("0123456789");
        primary.State.ShouldBeNull();
        addresses.Where(a => !a.IsDefault).ShouldHaveSingleItem().State.ShouldBe("Hanoi");
    }

    [Fact]
    public async Task FindAsync_ReturnsNullWhenOnlyIdentityUserExists()
    {
        // Arrange
        var id = await CreateIdentityUserAsync();

        // Act
        var result = await new UserRepository(Context).FindAsync(id, CancellationToken.None);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task FindAsync_ReturnsNullForUnknownId()
    {
        // Arrange
        // The fixture provides an empty database and a fresh context.

        // Act
        var result = await new UserRepository(Context).FindAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task Add_AllowsNullDisplayName()
    {
        // Arrange
        var id = await CreateIdentityUserAsync();

        // Act
        await new UserRepository(Context).Add(new User(id, id));
        await Context.SaveChangesAsync();

        // Assert
        await using var verification = CreateDbContext();
        var saved = await new UserRepository(verification).FindAsync(id, CancellationToken.None);
        saved.ShouldNotBeNull();
        saved.DisplayName.ShouldBeNull();
    }

    [Fact]
    public async Task Add_RejectsProfileWithoutIdentityUser()
    {
        // Arrange
        var id = Guid.NewGuid();
        await new UserRepository(Context).Add(new User(id, id, "Orphan"));

        // Act
        Func<Task> act = () => Context.SaveChangesAsync();

        // Assert
        var error = await Should.ThrowAsync<DbUpdateException>(act);
        error.InnerException.ShouldBeOfType<SqlException>().Number.ShouldBe(547);
    }

    [Fact]
    public async Task Add_RejectsMultipleDefaultAddressesForSameUser()
    {
        // Arrange
        var id = await CreateIdentityUserAsync();
        var user = new User(id, id);
        user.UpdateAddress(
        [
            new UserAddress(Guid.NewGuid(), id, id, "VN", "HCMC", "Street 1", "123", isDefault: true),
            new UserAddress(Guid.NewGuid(), id, id, "VN", "HCMC", "Street 2", "456", isDefault: true)
        ]);
        await new UserRepository(Context).Add(user);

        // Act
        Func<Task> act = () => Context.SaveChangesAsync();

        // Assert
        var error = await Should.ThrowAsync<DbUpdateException>(act);
        new[] { 2601, 2627 }.ShouldContain(error.InnerException.ShouldBeOfType<SqlException>().Number);
    }
}
