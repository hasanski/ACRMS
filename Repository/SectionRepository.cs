using ACRMS.Data;
using ACRMS.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ACRMS.Repository
{
    public class SectionRepository: ISectionRepository
    {
        private readonly ApplicationDbContext _context;

        public SectionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //public async Task<List<Section>> ListAsync()
        //{
        //    return await _context.Sections
        //        .AsNoTracking()
        //        .Include(s => s.Course)
        //        .ThenInclude(c => c!.Department)
        //        .OrderByDescending(s => s.Id)
        //        .ToListAsync();
        //}
        public async Task<List<Section>> ListAsync()
        {
            return await _context.Sections
                .Include(x => x.Course)
                    .ThenInclude(c => c!.Department)
                .Include(x => x.FacultyMember)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }
        //public async Task<List<ApplicationUser>> GetFacultyMembersAsync()
        //{
        //    return await _context.Users
        //        .Include(x => x.Department)
        //        .Where(x => x.IsActive)
        //        .Where(x => _context.UserRoles.Any(ur => ur.UserId == x.Id && ur.))
        //        .OrderBy(x => x.FullName)
        //        .ToListAsync();
        //}
        public async Task<List<ApplicationUser>> GetFacultyMembersAsync()
        {
            return await (
                from user in _context.Users
                join userRole in _context.UserRoles on user.Id equals userRole.UserId
                join role in _context.Roles on userRole.RoleId equals role.Id
                where user.IsActive && role.Name == "Faculty"
                select user
            )
            .Include(x => x.Department)
            .OrderBy(x => x.FullName)
            .ToListAsync();
        }

        public async Task<Section?> GetByIdAsync(int id)
        {
            return await _context.Sections
                .AsNoTracking()
                .Include(s => s.Course)
                .ThenInclude(c => c!.Department)
                .FirstOrDefaultAsync(s => s.Id == id);
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

        public async Task<bool> ExistsByNameInCourseAsync(string sectionName, int courseId, int? excludeId = null)
        {
            sectionName = sectionName.Trim();

            return await _context.Sections.AnyAsync(s =>
                s.SectionName == sectionName &&
                s.CourseId == courseId &&
                (!excludeId.HasValue || s.Id != excludeId.Value));
        }

        public async Task<bool> HasDependenciesAsync(int id)
        {
            var hasEnrollments = await _context.Enrollments.AnyAsync(e => e.SectionId == id);
            var hasAnnouncements = await _context.Announcements.AnyAsync(a => a.SectionId == id);
            var hasConversations = await _context.Conversations.AnyAsync(c => c.SectionId == id);

            return hasEnrollments || hasAnnouncements || hasConversations;
        }

        public async Task CreateAsync(Section section)
        {
            _context.Sections.Add(section);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Section section)
        {
            var existing = await _context.Sections.FindAsync(section.Id);

            if (existing is not null)
            {
                existing.SectionName = section.SectionName;
                existing.CourseId = section.CourseId;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var section = await _context.Sections.FindAsync(id);

            if (section is not null)
            {
                _context.Sections.Remove(section);
                await _context.SaveChangesAsync();
            }
        }
    }
}
