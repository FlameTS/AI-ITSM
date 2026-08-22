using AIITSM.Application._04_M4_Administration.DTOs;
using AIITSM.Application._04_M4_Administration.Interfaces;
using AIITSM.Domain._04_M4_Administration.Entities;
using AITSM.Infrastructure._01_M1_IdentityAccess.Identity;
using Microsoft.EntityFrameworkCore;

namespace AIITSM.Infrastructure._04_M4_Administration.Services;

public class CategoryAdministrationService : ICategoryAdministrationService
{
    private readonly ApplicationDbContext _dbContext;

    public CategoryAdministrationService(
        ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync()
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .OrderBy(c => c.CategoryName)
            .Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName
            })
            .ToListAsync();
    }

    public async Task<bool> CreateCategoryAsync(string categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            return false;
        }

        categoryName = categoryName.Trim();

        var exists = await _dbContext.Categories
            .AnyAsync(c => c.CategoryName == categoryName);

        if (exists)
        {
            return false;
        }

        var category = new Category
        {
            CategoryName = categoryName
        };

        _dbContext.Categories.Add(category);

        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateCategoryAsync(
        int categoryId,
        string categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            return false;
        }

        categoryName = categoryName.Trim();

        var category = await _dbContext.Categories
            .FirstOrDefaultAsync(c => c.CategoryId == categoryId);

        if (category == null)
        {
            return false;
        }

        var duplicateExists = await _dbContext.Categories
            .AnyAsync(c =>
                c.CategoryId != categoryId &&
                c.CategoryName == categoryName);

        if (duplicateExists)
        {
            return false;
        }

        category.CategoryName = categoryName;

        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteCategoryAsync(int categoryId)
    {
        var category = await _dbContext.Categories
            .FirstOrDefaultAsync(c => c.CategoryId == categoryId);

        if (category == null)
        {
            return false;
        }

        _dbContext.Categories.Remove(category);

        await _dbContext.SaveChangesAsync();

        return true;
    }
}