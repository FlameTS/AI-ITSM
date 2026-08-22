using AIITSM.Domain._04_M4_Administration.Entities;
using AIITSM.Infrastructure._04_M4_Administration.Services;
using AITSM.Infrastructure._01_M1_IdentityAccess.Identity;
using Microsoft.EntityFrameworkCore;

namespace AIITSM.M4.Tests;

public class CategoryAdministrationTests
{
    [Fact]
    public async Task CreateCategoryAsync_WithValidName_CreatesCategory()
    {
        await using var dbContext = CreateDbContext();

        var service = new CategoryAdministrationService(dbContext);

        var result = await service.CreateCategoryAsync("Hardware");

        Assert.True(result);

        var category = await dbContext.Categories
            .SingleOrDefaultAsync(c => c.CategoryName == "Hardware");

        Assert.NotNull(category);
    }

    [Fact]
    public async Task CreateCategoryAsync_WithDuplicateName_ReturnsFalse()
    {
        await using var dbContext = CreateDbContext();

        var service = new CategoryAdministrationService(dbContext);

        await service.CreateCategoryAsync("Hardware");

        var result = await service.CreateCategoryAsync("Hardware");

        Assert.False(result);

        var categoryCount = await dbContext.Categories
            .CountAsync(c => c.CategoryName == "Hardware");

        Assert.Equal(1, categoryCount);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public async Task CreateCategoryAsync_WithBlankName_ReturnsFalse(
        string categoryName)
    {
        await using var dbContext = CreateDbContext();

        var service = new CategoryAdministrationService(dbContext);

        var result = await service.CreateCategoryAsync(categoryName);

        Assert.False(result);
        Assert.Empty(dbContext.Categories);
    }

    [Fact]
    public async Task GetCategoriesAsync_ReturnsCategoriesOrderedByName()
    {
        await using var dbContext = CreateDbContext();

        dbContext.Categories.AddRange(
            new Category
            {
                CategoryName = "Software"
            },
            new Category
            {
                CategoryName = "Hardware"
            },
            new Category
            {
                CategoryName = "Network"
            });

        await dbContext.SaveChangesAsync();

        var service = new CategoryAdministrationService(dbContext);

        var result = await service.GetCategoriesAsync();

        Assert.Equal(3, result.Count);

        Assert.Equal("Hardware", result[0].CategoryName);
        Assert.Equal("Network", result[1].CategoryName);
        Assert.Equal("Software", result[2].CategoryName);
    }

    [Fact]
    public async Task UpdateCategoryAsync_WithValidName_UpdatesCategory()
    {
        await using var dbContext = CreateDbContext();

        var category = new Category
        {
            CategoryName = "Hardware"
        };

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        var service = new CategoryAdministrationService(dbContext);

        var result = await service.UpdateCategoryAsync(
            category.CategoryId,
            "Infrastructure");

        Assert.True(result);

        var updatedCategory = await dbContext.Categories
            .FindAsync(category.CategoryId);

        Assert.NotNull(updatedCategory);
        Assert.Equal("Infrastructure", updatedCategory.CategoryName);
    }

    [Fact]
    public async Task UpdateCategoryAsync_WithInvalidId_ReturnsFalse()
    {
        await using var dbContext = CreateDbContext();

        var service = new CategoryAdministrationService(dbContext);

        var result = await service.UpdateCategoryAsync(
            999,
            "Hardware");

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateCategoryAsync_WithDuplicateName_ReturnsFalse()
    {
        await using var dbContext = CreateDbContext();

        dbContext.Categories.AddRange(
            new Category
            {
                CategoryName = "Hardware"
            },
            new Category
            {
                CategoryName = "Software"
            });

        await dbContext.SaveChangesAsync();

        var softwareCategory = await dbContext.Categories
            .SingleAsync(c => c.CategoryName == "Software");

        var service = new CategoryAdministrationService(dbContext);

        var result = await service.UpdateCategoryAsync(
            softwareCategory.CategoryId,
            "Hardware");

        Assert.False(result);

        Assert.Equal(
            "Software",
            softwareCategory.CategoryName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public async Task UpdateCategoryAsync_WithBlankName_ReturnsFalse(
        string categoryName)
    {
        await using var dbContext = CreateDbContext();

        var category = new Category
        {
            CategoryName = "Hardware"
        };

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        var service = new CategoryAdministrationService(dbContext);

        var result = await service.UpdateCategoryAsync(
            category.CategoryId,
            categoryName);

        Assert.False(result);

        Assert.Equal(
            "Hardware",
            category.CategoryName);
    }

    [Fact]
    public async Task DeleteCategoryAsync_WithValidId_DeletesCategory()
    {
        await using var dbContext = CreateDbContext();

        var category = new Category
        {
            CategoryName = "Hardware"
        };

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        var service = new CategoryAdministrationService(dbContext);

        var result = await service.DeleteCategoryAsync(
            category.CategoryId);

        Assert.True(result);

        var deletedCategory = await dbContext.Categories
            .FindAsync(category.CategoryId);

        Assert.Null(deletedCategory);
    }

    [Fact]
    public async Task DeleteCategoryAsync_WithInvalidId_ReturnsFalse()
    {
        await using var dbContext = CreateDbContext();

        var service = new CategoryAdministrationService(dbContext);

        var result = await service.DeleteCategoryAsync(999);

        Assert.False(result);
    }

    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}