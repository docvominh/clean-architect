using CleanArchitect.Domain.UserAggregate;
using CleanArchitect.Infrastructure.IntegrationTests.Fixtures;
using CleanArchitect.Infrastructure.UserAggregate;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using Shouldly;

namespace CleanArchitect.Infrastructure.IntegrationTests.UserAggregate;

[Collection("MsSql")]
public sealed class RefreshTokenRepositoryTests(MsSqlContainerFixture fixture) : RepositoryTestBase(fixture)
{
    private RefreshTokenRepository Repository => new(Context);

    private static RefreshToken CreateToken(Guid userId, string token = "test-token")
    {
        return new RefreshToken(Guid.NewGuid(), userId, userId, token, DateTimeOffset.UtcNow.AddDays(7));
    }

    [Fact]
    public async Task AddAndSaveChanges_PersistsTokenAndFindsByValue()
    {
        // Arrange
        var id = await CreateIdentityUserAsync();
        var token = CreateToken(id);

        // Act
        Repository.Add(token);
        Repository.Add(CreateToken(id, "other-token"));
        bool wasMissingBeforeSave;
        await using (var beforeSave = CreateDbContext())
        {
            wasMissingBeforeSave = await new RefreshTokenRepository(beforeSave).FindAsync(token.Token, CancellationToken.None) is null;
        }

        await Repository.SaveChangesAsync(CancellationToken.None);

        // Assert
        wasMissingBeforeSave.ShouldBeTrue();
        await using var verification = CreateDbContext();
        var saved = await new RefreshTokenRepository(verification).FindAsync(token.Token, CancellationToken.None);
        saved.ShouldNotBeNull();
        saved.Id.ShouldBe(token.Id);
        saved.UserId.ShouldBe(id);
        saved.ExpiresAt.ShouldBe(token.ExpiresAt);
        saved.RevokedAt.ShouldBeNull();
        saved.ReplacedByToken.ShouldBeNull();
        saved.IsActive.ShouldBeTrue();
        saved.CreatedAt.ShouldNotBe(default);
    }

    [Fact]
    public async Task FindAsync_ReturnsNullForUnknownToken()
    {
        // Arrange
        // The fixture provides an empty database and a fresh context.

        // Act
        var result = await Repository.FindAsync("missing", CancellationToken.None);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_PersistsRevocationAndReplacement()
    {
        // Arrange
        var id = await CreateIdentityUserAsync();
        var token = CreateToken(id);
        Repository.Add(token);
        await Repository.SaveChangesAsync(CancellationToken.None);
        Context.ChangeTracker.Clear();
        var loaded = await Repository.FindAsync(token.Token, CancellationToken.None);
        loaded.ShouldNotBeNull();
        var revokedAt = DateTimeOffset.UtcNow;

        // Act
        loaded.RevokedAt = revokedAt;
        loaded.ReplacedByToken = "replacement";
        Repository.Add(CreateToken(id, "replacement"));
        await Repository.SaveChangesAsync(CancellationToken.None);

        // Assert
        await using var verification = CreateDbContext();
        var repository = new RefreshTokenRepository(verification);
        var saved = await repository.FindAsync(token.Token, CancellationToken.None);
        saved.ShouldNotBeNull();
        saved.RevokedAt.ShouldBe(revokedAt);
        saved.ReplacedByToken.ShouldBe("replacement");
        saved.IsActive.ShouldBeFalse();
        var replacement = await repository.FindAsync("replacement", CancellationToken.None);
        replacement.ShouldNotBeNull();
        replacement.IsActive.ShouldBeTrue();
    }

    [Fact]
    public async Task FindAsync_ReturnsExpiredTokenForCallerToValidate()
    {
        // Arrange
        var id = await CreateIdentityUserAsync();
        Repository.Add(new RefreshToken(Guid.NewGuid(), id, id, "expired", DateTimeOffset.UtcNow.AddDays(-1)));
        await Repository.SaveChangesAsync(CancellationToken.None);
        await using var verification = CreateDbContext();

        // Act
        var token = await new RefreshTokenRepository(verification).FindAsync("expired", CancellationToken.None);

        // Assert
        token.ShouldNotBeNull();
        token.IsActive.ShouldBeFalse();
    }

    [Fact]
    public async Task SaveChangesAsync_RejectsDuplicateTokenValuesAcrossUsers()
    {
        // Arrange
        var first = await CreateIdentityUserAsync();
        var second = await CreateIdentityUserAsync();
        Repository.Add(CreateToken(first));
        await Repository.SaveChangesAsync(CancellationToken.None);
        Repository.Add(CreateToken(second));

        // Act
        Func<Task> act = () => Repository.SaveChangesAsync(CancellationToken.None);

        // Assert
        var error = await Should.ThrowAsync<DbUpdateException>(act);
        new[] { 2601, 2627 }.ShouldContain(error.InnerException.ShouldBeOfType<SqlException>().Number);
    }

    [Fact]
    public async Task SaveChangesAsync_RejectsTokenWithoutIdentityUser()
    {
        // Arrange
        Repository.Add(CreateToken(Guid.NewGuid()));

        // Act
        Func<Task> act = () => Repository.SaveChangesAsync(CancellationToken.None);

        // Assert
        var error = await Should.ThrowAsync<DbUpdateException>(act);
        error.InnerException.ShouldBeOfType<SqlException>().Number.ShouldBe(547);
    }
}
