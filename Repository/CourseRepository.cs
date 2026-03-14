using ACRMS.Data;
using ACRMS.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ACRMS.Repository
{
    public class CourseRepository: ICourseRepository
    {
        private readonly ApplicationDbContext _context;

        public CourseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Course>> ListAsync()
        {
            return await _context.Courses
                .AsNoTracking()
                .Include(c => c.Department)
                .OrderByDescending(c => c.Id)
                .ToListAsync();
        }

        public async Task<Course?> GetByIdAsync(int id)
        {
            return await _context.Courses
                .AsNoTracking()
                .Include(c => c.Department)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Department>> GetDepartmentsAsync()
        {
            return await _context.Departments
                .AsNoTracking()
                .OrderBy(d => d.FacultyName)
                .ThenBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<bool> ExistsByCodeAsync(string code, int? excludeId = null)
        {
            code = code.Trim();

            return await _context.Courses.AnyAsync(c =>
                c.CourseCode == code &&
                (!excludeId.HasValue || c.Id != excludeId.Value));
        }

        public async Task<bool> HasDependenciesAsync(int id)
        {
            var hasSections = await _context.Sections.AnyAsync(s => s.CourseId == id);
            var hasGradeReviews = await _context.GradeReviewRequests.AnyAsync(g => g.CourseId == id);
            var hasEnrollments = await _context.Enrollments.AnyAsync(e => e.CourseId == id);

            return hasSections || hasGradeReviews || hasEnrollments;
        }

        public async Task CreateAsync(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Course course)
        {
            var existing = await _context.Courses.FindAsync(course.Id);

            if (existing is not null)
            {
                existing.CourseName = course.CourseName;
                existing.CourseCode = course.CourseCode;
                existing.DepartmentId = course.DepartmentId;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course is not null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
        }
    }
}