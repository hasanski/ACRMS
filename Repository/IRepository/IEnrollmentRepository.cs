using ACRMS.Data;

namespace ACRMS.Repository.IRepository
{
    public interface IEnrollmentRepository
    {
        Task<List<Enrollment>> ListAsync();

        Task<List<ApplicationUser>> GetStudentsAsync();
        Task<List<Department>> GetDepartmentsAsync();
        Task<List<Course>> GetCoursesAsync();
        Task<List<Section>> GetSectionsAsync();

        Task<bool> ExistsAsync(string studentId, int courseId, int? sectionId, int? excludeId = null);

        Task CreateAsync(Enrollment enrollment);
        Task DeleteAsync(int id);
    }
}
