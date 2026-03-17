using ACRMS.Data;
using ACRMS.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ACRMS.Repository
{
    public class UserManagementRepository: IUserManagementRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagementRepository(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<ApplicationUser>> ListUsersAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Include(u => u.Department)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Department>> GetDepartmentsAsync()
        {
            return await _context.Departments
                .AsNoTracking()
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<List<string>> GetRolesAsync()
        {
            return await _roleManager.Roles
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .Select(r => r.Name!)
                .ToListAsync();
        }

        public async Task<bool> EmailExistsAsync(string email, string? excludeUserId = null)
        {
            email = email.Trim().ToLower();

            return await _context.Users.AnyAsync(u =>
                u.Email!.ToLower() == email &&
                (excludeUserId == null || u.Id != excludeUserId));
        }

        public async Task<bool> UniversityNumberExistsAsync(string universityNumber, string? excludeUserId = null)
        {
            universityNumber = universityNumber.Trim();

            return await _context.Users.AnyAsync(u =>
                u.UniversityNumber == universityNumber &&
                (excludeUserId == null || u.Id != excludeUserId));
        }

        public async Task<(bool Succeeded, string ErrorMessage)> CreateUserAsync(
            string fullName,
            string email,
            string universityNumber,
            int? departmentId,
            string role,
            string password)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                return (false, "الدور المحدد غير موجود");

            var user = new ApplicationUser
            {
                FullName = fullName.Trim(),
                UserName = email.Trim(),
                Email = email.Trim(),
                UniversityNumber = universityNumber.Trim(),
                DepartmentId = departmentId,
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                return (false, errors);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(" | ", roleResult.Errors.Select(e => e.Description));
                return (false, errors);
            }

            return (true, string.Empty);
        }

        public async Task<(bool Succeeded, string ErrorMessage)> ToggleActiveAsync(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
                return (false, "المستخدم غير موجود");

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            return (true, string.Empty);
        }

        public async Task<string?> GetUserRoleAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault();
        }
    }
}
