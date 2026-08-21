using AIITSM.Application._04_M4_Administration.DTOs;

namespace AIITSM.Application._04_M4_Administration.Interfaces;

public interface IUserAdministrationService
{
    Task<IReadOnlyList<UserListDto>> GetUsersAsync();

    Task<UserListDto?> GetUserByIdAsync(string userId);

    Task<bool> SetUserActiveStatusAsync(
        string userId,
        bool isActive);

    Task<bool> AssignRoleAsync(
        string userId,
        string roleName);
}

