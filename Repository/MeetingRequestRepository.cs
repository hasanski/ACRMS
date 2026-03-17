using ACRMS.Data;
using ACRMS.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static ACRMS.Data.Enums;

namespace ACRMS.Repository
{
    public class MeetingRequestRepository: IMeetingRequestRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MeetingRequestRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<MeetingRequest>> ListAsync()
        {
            return await _context.MeetingRequests
                .AsNoTracking()
                .Include(m => m.Student)
                .Include(m => m.Faculty)
                .OrderByDescending(m => m.CreatedAt)
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
                .Include(u => u.Department)
                .Where(u => ids.Contains(u.Id))
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<string> GenerateReferenceNumberAsync()
        {
            var count = await _context.MeetingRequests.CountAsync() + 1;
            return $"MR-{DateTime.UtcNow:yyyyMMdd}-{count:D4}";
        }

        public async Task CreateAsync(MeetingRequest meetingRequest)
        {
            _context.MeetingRequests.Add(meetingRequest);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, RequestStatus status, string? facultyResponse)
        {
            var item = await _context.MeetingRequests.FindAsync(id);
            if (item is not null)
            {
                item.Status = status;
                item.FacultyResponse = string.IsNullOrWhiteSpace(facultyResponse) ? null : facultyResponse.Trim();
                item.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.MeetingRequests.FindAsync(id);
            if (item is not null)
            {
                _context.MeetingRequests.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<MeetingRequest>> GetStudentRequestsAsync(string studentId)
        {
            return await _context.MeetingRequests
                .AsNoTracking()
                .Include(m => m.Student)
                .Include(m => m.Faculty)
                .Where(m => m.StudentId == studentId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<MeetingRequest>> GetFacultyRequestsAsync(string facultyId)
        {
            return await _context.MeetingRequests
                .AsNoTracking()
                .Include(m => m.Student)
                .Include(m => m.Faculty)
                .Where(m => m.FacultyId == facultyId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }
    }
}
