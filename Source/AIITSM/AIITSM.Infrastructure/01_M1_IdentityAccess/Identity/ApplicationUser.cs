using Microsoft.AspNetCore.Identity;

namespace AITSM.Infrastructure._01_M1_IdentityAccess.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}