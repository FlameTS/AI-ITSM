using AIITSM.Application._04_M4_Administration.DTOs;
using AIITSM.Application._04_M4_Administration.Interfaces;
using AITSM.Infrastructure._01_M1_IdentityAccess.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AIITSM.Infrastructure._04_M4_Administration.Services;

public class UserAdministrationService : IUserAdministrationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public UserAdministrationService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IReadOnlyList<UserListDto>> GetUsersAsync()
    {
        var users = await _userManager.Users
            .AsNoTracking()
            .ToListAsync();

        var result = new List<UserListDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            result.Add(new UserListDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                IsActive = user.IsActive,
                Roles = roles
            });
        }

        return result;
    }

    public async Task<UserListDto?> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);

        return new UserListDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            IsActive = user.IsActive,
            Roles = roles
        };
    }

    public async Task<bool> SetUserActiveStatusAsync(
        string userId,
        bool isActive)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        user.IsActive = isActive;

        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded;
    }

    public async Task<bool> AssignRoleAsync(
        string userId,
        string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            return false;
        }

        if (await _userManager.IsInRoleAsync(user, roleName))
        {
            return true;
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);

        return result.Succeeded;
    }
}