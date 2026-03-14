using ACRMS.Data;

namespace ACRMS.Repository.IRepository
{
    public interface ICourseRepository
    {
        Task<List<Course>> ListAsync();
        Task<Course?> GetByIdAsync(int id);
        Task CreateAsync(Course course);
        Task UpdateAsync(Course course);
        Task DeleteAsync(int id);

        Task<bool> ExistsByCodeAsync(string code, int? excludeId = null);
        Task<bool> HasDependenciesAsync(int id);

        Task<List<Department>> GetDepartmentsAsync();
    }
}
