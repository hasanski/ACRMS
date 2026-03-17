using ACRMS.Data;
using ACRMS.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static ACRMS.Data.Enums;

namespace ACRMS.Repository
{
    public class AppointmentRepository: IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<Appointment>> ListAsync()
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Student)
                .Include(a => a.Faculty)
                .Include(a => a.MeetingRequest)
                .OrderByDescending(a => a.AppointmentDate)
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

        public async Task<List<MeetingRequest>> GetApprovedMeetingRequestsAsync()
        {
            return await _context.MeetingRequests
                .AsNoTracking()
                .Include(m => m.Student)
                .Include(m => m.Faculty)
                .Where(m => m.Status == RequestStatus.Approved)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(string studentId, string facultyId, DateTime appointmentDate, int? excludeId = null)
        {
            return await _context.Appointments.AnyAsync(a =>
                a.StudentId == studentId &&
                a.FacultyId == facultyId &&
                a.AppointmentDate == appointmentDate &&
                (!excludeId.HasValue || a.Id != excludeId.Value));
        }

        public async Task CreateAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Appointment appointment)
        {
            var existing = await _context.Appointments.FindAsync(appointment.Id);
            if (existing is not null)
            {
                existing.MeetingRequestId = appointment.MeetingRequestId;
                existing.StudentId = appointment.StudentId;
                existing.FacultyId = appointment.FacultyId;
                existing.AppointmentDate = appointment.AppointmentDate;
                existing.Location = appointment.Location;
                existing.MeetingType = appointment.MeetingType;
                existing.Status = appointment.Status;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.Appointments.FindAsync(id);
            if (item is not null)
            {
                _context.Appointments.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}