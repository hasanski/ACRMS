using ACRMS.Data;
using ACRMS.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static ACRMS.Data.Enums;

namespace ACRMS.Repository
{
    public class AnnouncementRepository: IAnnouncementRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AnnouncementRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<Announcement>> ListAsync()
        {
            return await _context.Announcements
                .AsNoTracking()
                .Include(a => a.Faculty)
                .Include(a => a.Course)
                    .ThenInclude(c => c!.Department)
                .Include(a => a.Section)
                .Include(a => a.TargetStudent)
                .OrderByDescending(a => a.PublishedAt)
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

        public async Task<List<ApplicationUser>> GetFacultyAsync()
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync("Faculty");
            var ids = usersInRole.Select(u => u.Id).ToList();

            return await _context.Users
                .AsNoTracking()
                .Where(u => ids.Contains(u.Id))
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task CreateAsync(Announcement announcement)
        {
            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.Announcements.FindAsync(id);
            if (item is not null)
            {
                _context.Announcements.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<ApplicationUser>> ResolveRecipientsAsync(
            AnnouncementTargetType targetType,
            int? departmentId,
            int? courseId,
            int? sectionId,
            string? targetStudentId)
        {
            var students = await GetStudentsAsync();
            var query = students.AsQueryable();

            switch (targetType)
            {
                case AnnouncementTargetType.AllStudents:
                    return query.ToList();

                case AnnouncementTargetType.ByDepartment:
                    return query.Where(s => s.DepartmentId == departmentId).ToList();

                case AnnouncementTargetType.ByCourse:
                    if (!courseId.HasValue) return new List<ApplicationUser>();

                    var courseStudentIds = await _context.Enrollments
                        .AsNoTracking()
                        .Where(e => e.CourseId == courseId.Value)
                        .Select(e => e.StudentId)
                        .Distinct()
                        .ToListAsync();

                    return query.Where(s => courseStudentIds.Contains(s.Id)).ToList();

                case AnnouncementTargetType.BySection:
                    if (!sectionId.HasValue) return new List<ApplicationUser>();

                    var sectionStudentIds = await _context.Enrollments
                        .AsNoTracking()
                        .Where(e => e.SectionId == sectionId.Value)
                        .Select(e => e.StudentId)
                        .Distinct()
                        .ToListAsync();

                    return query.Where(s => sectionStudentIds.Contains(s.Id)).ToList();

                case AnnouncementTargetType.IndividualStudent:
                    if (string.IsNullOrWhiteSpace(targetStudentId)) return new List<ApplicationUser>();
                    return query.Where(s => s.Id == targetStudentId).ToList();

                default:
                    return new List<ApplicationUser>();
            }
        }

        public async Task CreateRecipientsAsync(int announcementId, List<string> studentIds)
        {
            var recipients = studentIds
                .Distinct()
                .Select(id => new AnnouncementRecipient
                {
                    AnnouncementId = announcementId,
                    StudentId = id
                })
                .ToList();

            _context.AnnouncementRecipients.AddRange(recipients);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AnnouncementRecipient>> GetStudentAnnouncementsAsync(string studentId)
        {
            return await _context.AnnouncementRecipients
                .AsNoTracking()
                .Include(ar => ar.Announcement)
                    .ThenInclude(a => a.Faculty)
                .Include(ar => ar.Announcement)
                    .ThenInclude(a => a.Course)
                .Include(ar => ar.Announcement)
                    .ThenInclude(a => a.Section)
                .Where(ar => ar.StudentId == studentId)
                .OrderByDescending(ar => ar.Announcement!.PublishedAt)
                .ToListAsync();
        }

        public async Task MarkAnnouncementAsReadAsync(int recipientId)
        {
            var item = await _context.AnnouncementRecipients.FirstOrDefaultAsync(x => x.Id == recipientId);

            if (item is not null && !item.IsRead)
            {
                item.IsRead = true;
                item.ReadAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

    }
}
