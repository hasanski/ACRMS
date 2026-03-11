using ACRMS.Data;

namespace ACRMS.Repository.IRepository
{
    public interface IDepartmentRepository
    {
        Task<List<Department>> ListAsync();
        Task<Department?> GetByIdAsync(int id);
        Task CreateAsync(Department department);
        Task UpdateAsync(Department department);
        Task DeleteAsync(int id);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
        Task<bool> HasCoursesAsync(int id);
    }
}
