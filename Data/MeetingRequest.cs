using System.ComponentModel.DataAnnotations;
using static ACRMS.Data.Enums;

namespace ACRMS.Data
{
    public class MeetingRequest : BaseEntity
    {
        [Required, MaxLength(50)]
        public string ReferenceNumber { get; set; } = string.Empty;

        [Required]
        public string StudentId { get; set; } = string.Empty;
        public ApplicationUser? Student { get; set; }

        [Required]
        public string FacultyId { get; set; } = string.Empty;
        public ApplicationUser? Faculty { get; set; }

        [Required, MaxLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public DateTime PreferredDate { get; set; }

        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        public string? FacultyResponse { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
