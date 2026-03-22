using ACRMS.Data;
using ACRMS.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static ACRMS.Data.Enums;

namespace ACRMS.Repository
{
    public class OfficialExcuseRepository: IOfficialExcuseRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OfficialExcuseRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<OfficialExcuse>> GetStudentExcusesAsync(string studentId)
        {
            return await _context.OfficialExcuses
                .AsNoTracking()
                .Include(o => o.Student)
                .Include(o => o.Faculty)
                .Where(o => o.StudentId == studentId)
                .OrderByDescending(o => o.SubmittedAt)
                .ToListAsync();
        }

        public async Task<List<OfficialExcuse>> GetFacultyExcusesAsync(string facultyId)
        {
            return await _context.OfficialExcuses
                .AsNoTracking()
                .Include(o => o.Student)
                .Include(o => o.Faculty)
                .Where(o => o.FacultyId == facultyId)
                .OrderByDescending(o => o.SubmittedAt)
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
            var count = await _context.OfficialExcuses.CountAsync() + 1;
            return $"EX-{DateTime.UtcNow:yyyyMMdd}-{count:D4}";
        }

        public async Task CreateAsync(OfficialExcuse officialExcuse)
        {
            _context.OfficialExcuses.Add(officialExcuse);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, RequestStatus status, string? reviewerNote)
        {
            var item = await _context.OfficialExcuses.FindAsync(id);
            if (item is not null)
            {
                item.Status = status;
                item.ReviewerNote = string.IsNullOrWhiteSpace(reviewerNote) ? null : reviewerNote.Trim();
                item.ReviewedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.OfficialExcuses.FindAsync(id);
            if (item is not null)
            {
                _context.OfficialExcuses.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
