using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace VolunteerManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Basic profile details for display and audit
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be exactly 10 digits")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits")]
        public string? Phone { get; set; }

        // Account creation timestamp (UTC)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual NGOProfile? NGOProfile { get; set; }
        public virtual ICollection<VolunteerApplication> VolunteerApplications { get; set; } = new List<VolunteerApplication>();

        // Convenience computed property
        public string FullName => $"{FirstName} {LastName}";
    }
}
