using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VolunteerManagementSystem.Models
{
    public class Event
    {
        // Primary key
        public int Id { get; set; }

        // Foreign key to owning NGO profile
        public int NGOId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required]
        public DateTime StartAt { get; set; }

        [Required]
        public DateTime EndAt { get; set; }

        [Required]
        [StringLength(500)]
        public string Venue { get; set; } = string.Empty;

        [Required]
        public int Capacity { get; set; }

        public bool IsPublic { get; set; } = true;

        [StringLength(200)]
        public string? ImagePath { get; set; }

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [StringLength(20)]
        public string? ContactPhone { get; set; }

        [EmailAddress]
        [StringLength(256)]
        public string? ContactEmail { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("NGOId")]
        public virtual NGOProfile NGO { get; set; } = null!;

        public virtual ICollection<VolunteerApplication> VolunteerApplications { get; set; } = new List<VolunteerApplication>();

        // Computed properties for UI/business logic
        public int CurrentVolunteerCount => VolunteerApplications.Count(va => va.Status == ApplicationStatus.Approved);
        public bool IsFull => CurrentVolunteerCount >= Capacity;
        public bool IsUpcoming => StartAt > DateTime.UtcNow;
        public bool IsOngoing => StartAt <= DateTime.UtcNow && EndAt >= DateTime.UtcNow;
        public bool IsCompleted => EndAt < DateTime.UtcNow;
    }

    public enum ApplicationStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        Cancelled = 3
    }
}
