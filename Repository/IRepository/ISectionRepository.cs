using ACRMS.Data;

namespace ACRMS.Repository.IRepository
{
    public interface ISectionRepository
    {
        Task<List<Section>> ListAsync();
        Task<Section?> GetByIdAsync(int id);
        Task CreateAsync(Section section);
        Task UpdateAsync(Section section);
        Task DeleteAsync(int id);

        Task<bool> ExistsByNameInCourseAsync(string sectionName, int courseId, int? excludeId = null);
        Task<bool> HasDependenciesAsync(int id);

        Task<List<Department>> GetDepartmentsAsync();
        Task<List<Course>> GetCoursesAsync();
    }
}
