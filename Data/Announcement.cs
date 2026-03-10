using System.ComponentModel.DataAnnotations;
using static ACRMS.Data.Enums;

namespace ACRMS.Data
{
    public class Announcement: BaseEntity
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public string FacultyId { get; set; } = string.Empty;
        public ApplicationUser? Faculty { get; set; }

        public int? CourseId { get; set; }
        public Course? Course { get; set; }

        public int? SectionId { get; set; }
        public Section? Section { get; set; }

        public AnnouncementTargetType TargetType { get; set; }

        public string? TargetStudentId { get; set; }
        public ApplicationUser? TargetStudent { get; set; }

        public AnnouncementPriority Priority { get; set; } = AnnouncementPriority.Normal;

        [MaxLength(500)]
        public string? AttachmentPath { get; set; }

        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpireAt { get; set; }

        public bool IsImportant { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<AnnouncementRecipient> Recipients { get; set; } = new List<AnnouncementRecipient>();
    }
}
