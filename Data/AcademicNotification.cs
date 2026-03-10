using System.ComponentModel.DataAnnotations;
using static ACRMS.Data.Enums;

namespace ACRMS.Data
{
    public class AcademicNotification : BaseEntity
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        [Required]
        public string FacultyId { get; set; } = string.Empty;
        public ApplicationUser? Faculty { get; set; }

        [Required]
        public string StudentId { get; set; } = string.Empty;
        public ApplicationUser? Student { get; set; }

        public NotificationType Type { get; set; } = NotificationType.General;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }
    }
}
