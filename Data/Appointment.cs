using System.ComponentModel.DataAnnotations;
using static ACRMS.Data.Enums;

namespace ACRMS.Data
{
    public class Appointment : BaseEntity
    {
        public int MeetingRequestId { get; set; }
        public MeetingRequest? MeetingRequest { get; set; }

        [Required]
        public string StudentId { get; set; } = string.Empty;
        public ApplicationUser? Student { get; set; }

        [Required]
        public string FacultyId { get; set; } = string.Empty;
        public ApplicationUser? Faculty { get; set; }

        public DateTime AppointmentDate { get; set; }

        [MaxLength(250)]
        public string? Location { get; set; }

        public MeetingType MeetingType { get; set; } = MeetingType.InPerson;

        public RequestStatus Status { get; set; } = RequestStatus.Approved;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
