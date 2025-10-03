using System.ComponentModel.DataAnnotations;

namespace VolunteerManagementSystem.Models.ViewModels
{
    public class CreateNGOProfileViewModel
    {
        // Organization basic information captured when creating NGO profile
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
        [StringLength(20)]
        [Display(Name = "Contact Phone")]
        public string ContactPhone { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        [Display(Name = "Address")]
        public string Address { get; set; } = string.Empty;

        [StringLength(2000)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        // Optional logo upload
        [Display(Name = "Logo")]
        public IFormFile? LogoFile { get; set; }
    }
}
