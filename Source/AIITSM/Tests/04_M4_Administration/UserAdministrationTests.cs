using AIITSM.Infrastructure._04_M4_Administration.Services;
using AITSM.Infrastructure._01_M1_IdentityAccess.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AIITSM.M4.Tests;

public class UserAdministrationTests
{
    [Fact]
    public async Task GetUserByIdAsync_WithExistingUser_ReturnsUser()
    {
        await using var provider = CreateServiceProvider();

        var userManager =
            provider.GetRequiredService<UserManager<ApplicationUser>>();

        var roleManager =
            provider.GetRequiredService<RoleManager<ApplicationRole>>();

        var user = await CreateUserAsync(
            userManager,
            "user@test.com",
            "Test User");

        var service = new UserAdministrationService(
            userManager,
            roleManager);

        var result = await service.GetUserByIdAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal("Test User", result.FullName);
        Assert.Equal("user@test.com", result.Email);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithInvalidUserId_ReturnsNull()
    {
        await using var provider = CreateServiceProvider();

        var userManager =
            provider.GetRequiredService<UserManager<ApplicationUser>>();

        var roleManager =
            provider.GetRequiredService<RoleManager<ApplicationRole>>();

        var service = new UserAdministrationService(
            userManager,
            roleManager);

        var result = await service.GetUserByIdAsync(
            "invalid-user-id");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetUsersAsync_ReturnsUsers()
    {
        await using var provider = CreateServiceProvider();

        var userManager =
            provider.GetRequiredService<UserManager<ApplicationUser>>();

        var roleManager =
            provider.GetRequiredService<RoleManager<ApplicationRole>>();

        var user = await CreateUserAsync(
            userManager,
            "user@test.com",
            "Test User");

        var service = new UserAdministrationService(
            userManager,
            roleManager);

        var result = await service.GetUsersAsync();

        Assert.Single(result);

        var returnedUser = result[0];

        Assert.Equal(user.Id, returnedUser.UserId);
        Assert.Equal("Test User", returnedUser.FullName);
        Assert.Equal("user@test.com", returnedUser.Email);
        Assert.True(returnedUser.IsActive);
    }

    [Fact]
    public async Task SetUserActiveStatusAsync_WithExistingUser_UpdatesStatus()
    {
        await using var provider = CreateServiceProvider();

        var userManager =
            provider.GetRequiredService<UserManager<ApplicationUser>>();

        var roleManager =
            provider.GetRequiredService<RoleManager<ApplicationRole>>();

        var user = await CreateUserAsync(
            userManager,
            "user@test.com",
            "Test User");

        var service = new UserAdministrationService(
            userManager,
            roleManager);

        var result = await service.SetUserActiveStatusAsync(
            user.Id,
            false);

        Assert.True(result);

        var updatedUser =
            await userManager.FindByIdAsync(user.Id);

        Assert.NotNull(updatedUser);
        Assert.False(updatedUser.IsActive);
    }

    [Fact]
    public async Task SetUserActiveStatusAsync_WithInvalidUserId_ReturnsFalse()
    {
        await using var provider = CreateServiceProvider();

        var userManager =
            provider.GetRequiredService<UserManager<ApplicationUser>>();

        var roleManager =
            provider.GetRequiredService<RoleManager<ApplicationRole>>();

        var service = new UserAdministrationService(
            userManager,
            roleManager);

        var result = await service.SetUserActiveStatusAsync(
            "invalid-user-id",
            false);

        Assert.False(result);
    }

    [Fact]
    public async Task AssignRoleAsync_WithExistingUserAndRole_AssignsRole()
    {
        await using var provider = CreateServiceProvider();

        var userManager =
            provider.GetRequiredService<UserManager<ApplicationUser>>();

        var roleManager =
            provider.GetRequiredService<RoleManager<ApplicationRole>>();

        var user = await CreateUserAsync(
            userManager,
            "user@test.com",
            "Test User");

        await roleManager.CreateAsync(
            new ApplicationRole
            {
                Name = "ITAdministrator"
            });

        var service = new UserAdministrationService(
            userManager,
            roleManager);

        var result = await service.AssignRoleAsync(
            user.Id,
            "ITAdministrator");

        Assert.True(result);

        var roles =
            await userManager.GetRolesAsync(user);

        Assert.Contains("ITAdministrator", roles);
    }

    [Fact]
    public async Task AssignRoleAsync_WithInvalidUserId_ReturnsFalse()
    {
        await using var provider = CreateServiceProvider();

        var userManager =
            provider.GetRequiredService<UserManager<ApplicationUser>>();

        var roleManager =
            provider.GetRequiredService<RoleManager<ApplicationRole>>();

        await roleManager.CreateAsync(
            new ApplicationRole
            {
                Name = "ITAdministrator"
            });

        var service = new UserAdministrationService(
            userManager,
            roleManager);

        var result = await service.AssignRoleAsync(
            "invalid-user-id",
            "ITAdministrator");

        Assert.False(result);
    }

    [Fact]
    public async Task AssignRoleAsync_WithInvalidRole_ReturnsFalse()
    {
        await using var provider = CreateServiceProvider();

        var userManager =
            provider.GetRequiredService<UserManager<ApplicationUser>>();

        var roleManager =
            provider.GetRequiredService<RoleManager<ApplicationRole>>();

        var user = await CreateUserAsync(
            userManager,
            "user@test.com",
            "Test User");

        var service = new UserAdministrationService(
            userManager,
            roleManager);

        var result = await service.AssignRoleAsync(
            user.Id,
            "NonExistingRole");

        Assert.False(result);
    }

    [Fact]
    public async Task AssignRoleAsync_WhenUserAlreadyHasRole_ReturnsTrue()
    {
        await using var provider = CreateServiceProvider();

        var userManager =
            provider.GetRequiredService<UserManager<ApplicationUser>>();

        var roleManager =
            provider.GetRequiredService<RoleManager<ApplicationRole>>();

        var user = await CreateUserAsync(
            userManager,
            "user@test.com",
            "Test User");

        await roleManager.CreateAsync(
            new ApplicationRole
            {
                Name = "ITAdministrator"
            });

        await userManager.AddToRoleAsync(
            user,
            "ITAdministrator");

        var service = new UserAdministrationService(
            userManager,
            roleManager);

        var result = await service.AssignRoleAsync(
            user.Id,
            "ITAdministrator");

        Assert.True(result);

        var roles =
            await userManager.GetRolesAsync(user);

        Assert.Single(roles);
        Assert.Contains("ITAdministrator", roles);
    }

    private static ServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddDbContext<ApplicationDbContext>(
            options =>
                options.UseInMemoryDatabase(
                    Guid.NewGuid().ToString()));

        services.AddIdentityCore<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        return services.BuildServiceProvider();
    }

    private static async Task<ApplicationUser> CreateUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string fullName)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            IsActive = true
        };

        var result = await userManager.CreateAsync(
            user,
            "Test@12345");

        Assert.True(
            result.Succeeded,
            string.Join(
                ", ",
                result.Errors.Select(
                    error => error.Description)));

        return user;
    }
}