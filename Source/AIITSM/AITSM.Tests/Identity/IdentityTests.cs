using AITSM.Infrastructure._01_M1_IdentityAccess.Identity;

namespace AITSM.Tests.Identity
{
    public class IdentityTests
    {
        [Fact]
        public void ApplicationUser_ShouldBeActiveByDefault()
        {
            // Arrange
            var user = new ApplicationUser
            {
                FullName = "Test User",
                Email = "test@aitsm.com",
                UserName = "test@aitsm.com"
            };

            // Act
            var isActive = user.IsActive;

            // Assert
            Assert.True(isActive);
        }
        [Fact]
        public void ApplicationRole_ShouldStoreRoleName()
        {
            // Arrange
            var role = new ApplicationRole
            {
                Name = "ITAdministrator"
            };

            // Assert
            Assert.Equal("ITAdministrator", role.Name);
        }
        [Fact]
        public void ApplicationUser_ShouldStoreFullNameAndEmail()
        {
            // Arrange
            var user = new ApplicationUser
            {
                FullName = "Test Employee",
                Email = "employee@aitsm.com",
                UserName = "employee@aitsm.com"
            };

            // Assert
            Assert.Equal("Test Employee", user.FullName);
            Assert.Equal("employee@aitsm.com", user.Email);
            Assert.Equal("employee@aitsm.com", user.UserName);
        }
        [Fact]
        public async Task UserManager_ShouldCreateUserSuccessfully()
        {
            // Arrange
            using var serviceProvider =
                IdentityTestHelper.CreateServiceProvider();

            var userManager =
                IdentityTestHelper.GetUserManager(serviceProvider);

            var user = new ApplicationUser
            {
                FullName = "Test User",
                UserName = "testuser@aitsm.com",
                Email = "testuser@aitsm.com",
                IsActive = true
            };

            // Act
            var result = await userManager.CreateAsync(
                user,
                "Test@123");

            // Assert
            Assert.True(result.Succeeded);

            var savedUser =
                await userManager.FindByEmailAsync("testuser@aitsm.com");

            Assert.NotNull(savedUser);
            Assert.Equal("Test User", savedUser.FullName);
        }
        [Fact]
        public async Task User_ShouldBeAssignedToEmployeeRole()
        {
            // Arrange
            using var serviceProvider =
                IdentityTestHelper.CreateServiceProvider();

            var userManager =
                IdentityTestHelper.GetUserManager(serviceProvider);

            var roleManager =
                IdentityTestHelper.GetRoleManager(serviceProvider);

            await roleManager.CreateAsync(
                new ApplicationRole
                {
                    Name = "Employee"
                });

            var user = new ApplicationUser
            {
                FullName = "Employee User",
                UserName = "employee@test.com",
                Email = "employee@test.com",
                IsActive = true
            };

            await userManager.CreateAsync(
                user,
                "Employee@123");

            // Act
            var result =
                await userManager.AddToRoleAsync(user, "Employee");

            // Assert
            Assert.True(result.Succeeded);

            var isEmployee =
                await userManager.IsInRoleAsync(user, "Employee");

            Assert.True(isEmployee);
        }
        [Fact]
        public async Task UserManager_ShouldRejectDuplicateEmail()
        {
            using var serviceProvider =
                IdentityTestHelper.CreateServiceProvider();

            var userManager =
                IdentityTestHelper.GetUserManager(serviceProvider);

            var firstUser = new ApplicationUser
            {
                FullName = "First User",
                UserName = "first@aitsm.com",
                Email = "duplicate@aitsm.com",
                IsActive = true
            };

            var secondUser = new ApplicationUser
            {
                FullName = "Second User",
                UserName = "second@aitsm.com",
                Email = "duplicate@aitsm.com",
                IsActive = true
            };

            var firstResult = await userManager.CreateAsync(
                firstUser,
                "First@123");

            var secondResult = await userManager.CreateAsync(
                secondUser,
                "Second@123");

            Assert.True(firstResult.Succeeded);
            Assert.False(secondResult.Succeeded);
        }
        [Fact]
        public async Task PasswordReset_ShouldReplaceOldPassword()
        {
            using var serviceProvider =
                IdentityTestHelper.CreateServiceProvider();

            var userManager =
                IdentityTestHelper.GetUserManager(serviceProvider);

            var user = new ApplicationUser
            {
                FullName = "Password Test User",
                UserName = "passwordtest@aitsm.com",
                Email = "passwordtest@aitsm.com",
                IsActive = true
            };

            var createResult = await userManager.CreateAsync(
                user,
                "OldPassword@123");

            Assert.True(createResult.Succeeded);

            var token =
                await userManager.GeneratePasswordResetTokenAsync(user);

            var resetResult =
                await userManager.ResetPasswordAsync(
                    user,
                    token,
                    "NewPassword@123");

            Assert.True(resetResult.Succeeded);

            var oldPasswordWorks =
                await userManager.CheckPasswordAsync(
                    user,
                    "OldPassword@123");

            var newPasswordWorks =
                await userManager.CheckPasswordAsync(
                    user,
                    "NewPassword@123");

            Assert.False(oldPasswordWorks);
            Assert.True(newPasswordWorks);
        }
        [Fact]
        public async Task DeactivatedUser_ShouldRemainInactive()
        {
            using var serviceProvider =
                IdentityTestHelper.CreateServiceProvider();

            var userManager =
                IdentityTestHelper.GetUserManager(serviceProvider);

            var user = new ApplicationUser
            {
                FullName = "Inactive User",
                UserName = "inactive@aitsm.com",
                Email = "inactive@aitsm.com",
                IsActive = true
            };

            var createResult = await userManager.CreateAsync(
                user,
                "Inactive@123");

            Assert.True(createResult.Succeeded);

            user.IsActive = false;

            var updateResult =
                await userManager.UpdateAsync(user);

            Assert.True(updateResult.Succeeded);

            var savedUser =
                await userManager.FindByEmailAsync(
                    "inactive@aitsm.com");

            Assert.NotNull(savedUser);
            Assert.False(savedUser.IsActive);
        }
    }
}