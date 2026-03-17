using ACRMS.Data;
using ACRMS.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ACRMS.Repository
{
    public class EnrollmentRepository: IEnrollmentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EnrollmentRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<Enrollment>> ListAsync()
        {
            return await _context.Enrollments
                .AsNoTracking()
                .Include(e => e.Student)
                .Include(e => e.Course)
                    .ThenInclude(c => c!.Department)
                .Include(e => e.Section)
                .OrderByDescending(e => e.Id)
                .ToListAsync();
        }

        public async Task<List<ApplicationUser>> GetStudentsAsync()
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync("Student");
            var ids = usersInRole.Select(u => u.Id).ToList();

            return await _context.Users
                .AsNoTracking()
                .Include(u => u.Department)
                .Where(u => ids.Contains(u.Id))
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<List<Department>> GetDepartmentsAsync()
        {
            return await _context.Departments
                .AsNoTracking()
                .OrderBy(d => d.FacultyName)
                .ThenBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<List<Course>> GetCoursesAsync()
        {
            return await _context.Courses
                .AsNoTracking()
                .Include(c => c.Department)
                .OrderBy(c => c.CourseName)
                .ToListAsync();
        }

        public async Task<List<Section>> GetSectionsAsync()
        {
            return await _context.Sections
                .AsNoTracking()
                .Include(s => s.Course)
                    .ThenInclude(c => c!.Department)
                .OrderBy(s => s.SectionName)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(string studentId, int courseId, int? sectionId, int? excludeId = null)
        {
            return await _context.Enrollments.AnyAsync(e =>
                e.StudentId == studentId &&
                e.CourseId == courseId &&
                e.SectionId == sectionId &&
                (!excludeId.HasValue || e.Id != excludeId.Value));
        }

        public async Task CreateAsync(Enrollment enrollment)
        {
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment is not null)
            {
                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
            }
        }
    }
}
