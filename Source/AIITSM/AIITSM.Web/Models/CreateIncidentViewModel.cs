using System.ComponentModel.DataAnnotations;
using AIITSM.Application._02_M2_IncidentManagement;
using AIITSM.Domain._02_M2_IncidentManagement;

namespace AIITSM.Web.Models
{
    public class CreateIncidentViewModel
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a category.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a category.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Please select a priority.")]
        public IncidentPriority Priority { get; set; } = IncidentPriority.Medium;

        // Populated by the controller so the view can render the dropdown.
        public IEnumerable<CategoryDto> Categories { get; set; } = Array.Empty<CategoryDto>();
    }
}
