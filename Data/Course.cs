using System.ComponentModel.DataAnnotations;
using static System.Collections.Specialized.BitVector32;

namespace ACRMS.Data
{
    public class Course : BaseEntity
    {
        [Required, MaxLength(200)]
        public string CourseName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string CourseCode { get; set; } = string.Empty;

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        public string? FacultyMemberId { get; set; }
        public ApplicationUser? FacultyMember { get; set; }

        public ICollection<Section> Sections { get; set; } = new List<Section>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
        public ICollection<GradeReviewRequest> GradeReviewRequests { get; set; } = new List<GradeReviewRequest>();
        public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
    }
}
