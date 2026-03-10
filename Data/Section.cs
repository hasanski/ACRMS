using System.ComponentModel.DataAnnotations;

namespace ACRMS.Data
{
    public class Section: BaseEntity
    {
        [Required, MaxLength(100)]
        public string SectionName { get; set; } = string.Empty;

        public int CourseId { get; set; }
        public Course? Course { get; set; }

        public string? FacultyMemberId { get; set; }
        public ApplicationUser? FacultyMember { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
        public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
    }
}
