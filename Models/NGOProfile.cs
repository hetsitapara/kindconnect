using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace VolunteerManagementSystem.Models
{
    public class NGOProfile
    {
        // Primary key
        public int Id { get; set; }

        // Owner user id (one-to-one with ApplicationUser)
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Mission { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string ContactEmail { get; set; } = string.Empty;

        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be exactly 10 digits")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits")]
        public string? ContactPhone { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(200)]
        public string? LogoPath { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        public virtual ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
