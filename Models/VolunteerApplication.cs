using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VolunteerManagementSystem.Models
{
    public class VolunteerApplication
    {
        // Primary key
        public int Id { get; set; }

        // Event being applied to
        [Required]
        public int EventId { get; set; }

        // Applicant user id
        [Required]
        public string UserId { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Message { get; set; }

        [StringLength(500)]
        public string? Skills { get; set; }

        [StringLength(500)]
        public string? Availability { get; set; }

        // Lifecycle status managed by NGO/Superuser
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        public DateTime? RespondedAt { get; set; }

        [StringLength(500)]
        public string? ResponseMessage { get; set; }

        public string? RespondedByUserId { get; set; }

        // Navigation properties
        [ForeignKey("EventId")]
        public virtual Event Event { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        [ForeignKey("RespondedByUserId")]
        public virtual ApplicationUser? RespondedByUser { get; set; }
    }
}
