using System.ComponentModel.DataAnnotations;
using static ACRMS.Data.Enums;

namespace ACRMS.Data
{
    public class AcademicNote : BaseEntity
    {
        [Required]
        public string FacultyId { get; set; } = string.Empty;
        public ApplicationUser? Faculty { get; set; }

        [Required]
        public string StudentId { get; set; } = string.Empty;
        public ApplicationUser? Student { get; set; }

        [Required]
        public string NoteText { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public NoteVisibility Visibility { get; set; } = NoteVisibility.StudentAndFaculty;
    }
}
