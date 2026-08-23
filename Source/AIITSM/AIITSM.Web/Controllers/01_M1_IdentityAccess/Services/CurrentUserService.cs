using System.Security.Claims;
using AITSM.Application._01_M1_IdentityAccess.Interfaces;

namespace AIITSM.Web._01_M1_IdentityAccess.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User =>
            _httpContextAccessor.HttpContext?.User;

        public string? UserId =>
            User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public string? Email =>
            User?.FindFirstValue(ClaimTypes.Email)
            ?? User?.Identity?.Name;

        public bool IsAuthenticated =>
            User?.Identity?.IsAuthenticated ?? false;

        public bool IsInRole(string role)
        {
            return User?.IsInRole(role) ?? false;
        }
    }
}