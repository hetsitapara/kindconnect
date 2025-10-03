using System.ComponentModel.DataAnnotations;

namespace VolunteerManagementSystem.Models.ViewModels
{
    public class EditEventViewModel
    {
        public int Id { get; set; }

        // Editable fields mirror CreateEventViewModel
        [Required]
        [StringLength(200)]
        [Display(Name = "Event Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Category")]
        public string Category { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Start Date & Time")]
        public DateTime StartAt { get; set; }

        [Required]
        [Display(Name = "End Date & Time")]
        public DateTime EndAt { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Venue")]
        public string Venue { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0")]
        [Display(Name = "Volunteer Capacity")]
        public int Capacity { get; set; }

        [Display(Name = "Make this event public")]
        public bool IsPublic { get; set; } = true;

        [StringLength(200)]
        [Display(Name = "Contact Person")]
        public string? ContactPerson { get; set; }

        [StringLength(20)]
        [Display(Name = "Contact Phone")]
        public string? ContactPhone { get; set; }

        [EmailAddress]
        [StringLength(256)]
        [Display(Name = "Contact Email")]
        public string? ContactEmail { get; set; }

        // Read-only properties for display
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public int NGOId { get; set; }
    }
}
