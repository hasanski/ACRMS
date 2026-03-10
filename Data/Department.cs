using System.ComponentModel.DataAnnotations;

namespace ACRMS.Data
{
    public class Department : BaseEntity
    {
        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(150)]
        public string FacultyName { get; set; } = string.Empty;

        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
