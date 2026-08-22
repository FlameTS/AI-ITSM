using AIITSM.Application._04_M4_Administration.DTOs;

namespace AIITSM.Application._04_M4_Administration.Interfaces;

public interface ICategoryAdministrationService
{
    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync();

    Task<bool> CreateCategoryAsync(string categoryName);

    Task<bool> UpdateCategoryAsync(
        int categoryId,
        string categoryName);

    Task<bool> DeleteCategoryAsync(int categoryId);
}