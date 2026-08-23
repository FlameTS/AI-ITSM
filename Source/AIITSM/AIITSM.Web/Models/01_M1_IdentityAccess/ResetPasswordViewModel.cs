using System.ComponentModel.DataAnnotations;

namespace AIITSM.Web.Models._01_M1_IdentityAccess
{
    public class ResetPasswordViewModel
    {
        public string UserId { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}