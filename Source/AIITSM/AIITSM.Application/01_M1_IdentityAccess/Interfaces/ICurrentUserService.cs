namespace AITSM.Application._01_M1_IdentityAccess.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserId { get; }

        string? Email { get; }

        bool IsAuthenticated { get; }

        bool IsInRole(string role);
    }
}