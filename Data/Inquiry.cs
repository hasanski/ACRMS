using System.ComponentModel.DataAnnotations;
using static ACRMS.Data.Enums;

namespace ACRMS.Data
{
    public class Inquiry : BaseEntity
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
        public string Message { get; set; } = string.Empty;

        public InquiryStatus Status { get; set; } = InquiryStatus.Open;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RepliedAt { get; set; }

        public string? ReplyMessage { get; set; }
    }
}
