namespace AIITSM.Application._04_M4_Administration.DTOs;

public class UserListDto
{
    public string UserId { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public IList<string> Roles { get; set; } = new List<string>();
}