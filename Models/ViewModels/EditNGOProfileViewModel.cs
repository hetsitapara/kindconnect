using System.ComponentModel.DataAnnotations;

namespace VolunteerManagementSystem.Models.ViewModels
{
    public class EditNGOProfileViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Organization Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        [Display(Name = "Mission Statement")]
        public string Mission { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(256)]
        [Display(Name = "Contact Email")]
        public string ContactEmail { get; set; } = string.Empty;

        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be exactly 10 digits")]
        [Display(Name = "Contact Phone")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits")]
        public string ContactPhone { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        [Display(Name = "Address")]
        public string Address { get; set; } = string.Empty;

        [StringLength(2000)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Logo")]
        public IFormFile? LogoFile { get; set; }

        // Read-only properties for display
        public string? LogoPath { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
