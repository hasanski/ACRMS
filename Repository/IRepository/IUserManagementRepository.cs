using ACRMS.Data;

namespace ACRMS.Repository.IRepository
{
    public interface IUserManagementRepository
    {
        Task<List<ApplicationUser>> ListUsersAsync();
        Task<List<Department>> GetDepartmentsAsync();
        Task<List<string>> GetRolesAsync();

        Task<bool> EmailExistsAsync(string email, string? excludeUserId = null);
        Task<bool> UniversityNumberExistsAsync(string universityNumber, string? excludeUserId = null);

        Task<(bool Succeeded, string ErrorMessage)> CreateUserAsync(
            string fullName,
            string email,
            string universityNumber,
            int? departmentId,
            string role,
            string password);

        Task<(bool Succeeded, string ErrorMessage)> ToggleActiveAsync(string userId);
        Task<string?> GetUserRoleAsync(string userId);
    }
}
