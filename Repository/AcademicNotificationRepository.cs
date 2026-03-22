using ACRMS.Data;
using ACRMS.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ACRMS.Repository
{
    public class AcademicNotificationRepository: IAcademicNotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public AcademicNotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AcademicNotification>> GetStudentNotificationsAsync(string studentId)
        {
            return await _context.AcademicNotifications
                .AsNoTracking()
                .Include(n => n.Faculty)
                .Include(n => n.Student)
                .Where(n => n.StudentId == studentId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<AcademicNotification>> GetFacultyNotificationsAsync(string facultyId)
        {
            return await _context.AcademicNotifications
                .AsNoTracking()
                .Include(n => n.Faculty)
                .Include(n => n.Student)
                .Where(n => n.FacultyId == facultyId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task CreateAsync(AcademicNotification notification)
        {
            _context.AcademicNotifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task MarkAsReadAsync(int id)
        {
            var item = await _context.AcademicNotifications.FindAsync(id);
            if (item is not null && !item.IsRead)
            {
                item.IsRead = true;
                item.ReadAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadForStudentAsync(string studentId)
        {
            var items = await _context.AcademicNotifications
                .Where(n => n.StudentId == studentId && !n.IsRead)
                .ToListAsync();

            foreach (var item in items)
            {
                item.IsRead = true;
                item.ReadAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task MarkAllAsReadForFacultyAsync(string facultyId)
        {
            var items = await _context.AcademicNotifications
                .Where(n => n.FacultyId == facultyId && !n.IsRead)
                .ToListAsync();

            foreach (var item in items)
            {
                item.IsRead = true;
                item.ReadAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUnreadCountForStudentAsync(string studentId)
        {
            return await _context.AcademicNotifications
                .CountAsync(n => n.StudentId == studentId && !n.IsRead);
        }

        public async Task<int> GetUnreadCountForFacultyAsync(string facultyId)
        {
            return await _context.AcademicNotifications
                .CountAsync(n => n.FacultyId == facultyId && !n.IsRead);
        }
    }
}