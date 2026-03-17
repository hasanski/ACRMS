using ACRMS.Data;

namespace ACRMS.Repository.IRepository
{
    public interface IAppointmentRepository
    {
        Task<List<Appointment>> ListAsync();

        Task<List<ApplicationUser>> GetStudentsAsync();
        Task<List<ApplicationUser>> GetFacultyAsync();
        Task<List<MeetingRequest>> GetApprovedMeetingRequestsAsync();

        Task CreateAsync(Appointment appointment);
        Task UpdateAsync(Appointment appointment);
        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(string studentId, string facultyId, DateTime appointmentDate, int? excludeId = null);

    }
}
