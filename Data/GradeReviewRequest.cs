using System.ComponentModel.DataAnnotations;
using static ACRMS.Data.Enums;

namespace ACRMS.Data
{
    public class GradeReviewRequest : BaseEntity
    {
        [Required, MaxLength(50)]
        public string ReferenceNumber { get; set; } = string.Empty;

        [Required]
        public string StudentId { get; set; } = string.Empty;
        public ApplicationUser? Student { get; set; }

        [Required]
        public string FacultyId { get; set; } = string.Empty;
        public ApplicationUser? Faculty { get; set; }

        public int CourseId { get; set; }
        public Course? Course { get; set; }

        [Required, MaxLength(150)]
        public string AssessmentName { get; set; } = string.Empty;

        public decimal CurrentMark { get; set; }

        [Required]
        public string RequestReason { get; set; } = string.Empty;

        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }

        public string? FacultyNote { get; set; }
    }
}
