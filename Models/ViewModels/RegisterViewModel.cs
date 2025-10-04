using System.ComponentModel.DataAnnotations;

namespace VolunteerManagementSystem.Models.ViewModels
{
    public class RegisterViewModel
    {
        // Login credentials
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be exactly 10 digits")]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits")]
        public string? Phone { get; set; }

        // Role selection (Volunteer or NGO)
        [Required]
        [Display(Name = "I want to register as")]
        public string SelectedRole { get; set; } = string.Empty;

        // Additional fields for NGO registration
        [Display(Name = "Organization Name")]
        [StringLength(200, ErrorMessage = "The {0} must be at max {1} characters long.")]
        public string? OrganizationName { get; set; }

        [Display(Name = "Organization Mission")]
        [StringLength(1000, ErrorMessage = "The {0} must be at max {1} characters long.")]
        public string? OrganizationMission { get; set; }

        [Display(Name = "Organization Contact Email")]
        [EmailAddress]
        [StringLength(256, ErrorMessage = "The {0} must be at max {1} characters long.")]
        public string? OrganizationContactEmail { get; set; }

        [Display(Name = "Organization Contact Phone")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be exactly 10 digits")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits")]
        public string? OrganizationContactPhone { get; set; }

        [Display(Name = "Organization Address")]
        [StringLength(500, ErrorMessage = "The {0} must be at max {1} characters long.")]
        public string? OrganizationAddress { get; set; }

        [Display(Name = "Organization Description")]
        [StringLength(1000, ErrorMessage = "The {0} must be at max {1} characters long.")]
        public string? OrganizationDescription { get; set; }
    }
}
