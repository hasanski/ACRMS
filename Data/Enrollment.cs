namespace ACRMS.Data
{
    public class Enrollment:BaseEntity
    {
        public string StudentId { get; set; } = string.Empty;
        public ApplicationUser? Student { get; set; }

        public int CourseId { get; set; }
        public Course? Course { get; set; }

        public int? SectionId { get; set; }
        public Section? Section { get; set; }

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    }
}
