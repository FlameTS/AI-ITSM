using AIITSM.Web._01_M1_IdentityAccess.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AITSM.Tests.Identity
{
    public class CurrentUserServiceTests
    {
        [Fact]
        public void CurrentUserService_ShouldReturnLoggedInUserInformation()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user-123"),
                new Claim(ClaimTypes.Email, "employee@aitsm.com"),
                new Claim(ClaimTypes.Role, "Employee")
            };

            var identity = new ClaimsIdentity(
                claims,
                "TestAuthentication");

            var principal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext
            {
                User = principal
            };

            var accessor = new HttpContextAccessor
            {
                HttpContext = httpContext
            };

            var service = new CurrentUserService(accessor);

            // Assert
            Assert.True(service.IsAuthenticated);
            Assert.Equal("user-123", service.UserId);
            Assert.Equal("employee@aitsm.com", service.Email);
            Assert.True(service.IsInRole("Employee"));
            Assert.False(service.IsInRole("ITAdministrator"));
        }
    }
}