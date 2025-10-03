using System.ComponentModel.DataAnnotations;

namespace VolunteerManagementSystem.Models.ViewModels
{
    public class VolunteerApplicationViewModel
    {
        public int Id { get; set; }

        // Information provided by the volunteer when applying
        [Required]
        [StringLength(1000)]
        [Display(Name = "Why do you want to volunteer for this event?")]
        public string Message { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Relevant Skills & Experience")]
        public string? Skills { get; set; }

        [StringLength(500)]
        [Display(Name = "Availability")]
        public string? Availability { get; set; }

        // Read-only properties for display
        public int EventId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationStatus Status { get; set; }
        public DateTime AppliedAt { get; set; }
        public DateTime? RespondedAt { get; set; }
        public string? ResponseMessage { get; set; }
        public string? RespondedByUserId { get; set; }

        // Navigation properties for display
        public string? EventTitle { get; set; }
        public string? UserName { get; set; }
        public string? RespondedByUserName { get; set; }
    }
}
