using CleanArchitect.Application.UserAggregate;
using CleanArchitect.Domain.UserAggregate;
using CleanArchitect.Infrastructure;

using Microsoft.AspNetCore.Identity;

namespace CleanArchitect.Api;

public static class SetupAdminUser
{
    public static async Task SetupAdminUserAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        foreach (var role in new[] { "Admin", "User" })
            if (!await roleManager.RoleExistsAsync(role))
            {
                EnsureIdentitySuccess(await roleManager.CreateAsync(new IdentityRole<Guid>(role)));
            }

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        const string adminEmail = "admin@gmail.com";
        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin is null)
        {
            admin = new AppUser
            {
                Id = Guid.NewGuid(),
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            // Allow the requested bootstrap password without weakening the registration password policy.
            admin.PasswordHash = userManager.PasswordHasher.HashPassword(admin, "admin");
            EnsureIdentitySuccess(await userManager.CreateAsync(admin));
        }

        if (!await userManager.IsInRoleAsync(admin, "Admin"))
        {
            EnsureIdentitySuccess(await userManager.AddToRoleAsync(admin, "Admin"));
        }

        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        if (await userRepository.FindAsync(admin.Id, cancellationToken) is null)
        {
            await userRepository.Add(new User(admin.Id, admin.Id, "Admin"));
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private static void EnsureIdentitySuccess(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Identity startup seeding failed: {string.Join("; ", result.Errors.Select(error => $"{error.Code}: {error.Description}"))}");
        }
    }
}
