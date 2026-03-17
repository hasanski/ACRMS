using ACRMS.Data;
using static ACRMS.Data.Enums;

namespace ACRMS.Repository.IRepository
{
    public interface IMeetingRequestRepository
    {
        Task<List<MeetingRequest>> ListAsync();

        Task<List<ApplicationUser>> GetStudentsAsync();
        Task<List<ApplicationUser>> GetFacultyAsync();

        Task<string> GenerateReferenceNumberAsync();

        Task CreateAsync(MeetingRequest meetingRequest);
        Task UpdateStatusAsync(int id, RequestStatus status, string? facultyResponse);
        Task DeleteAsync(int id);

        Task<List<MeetingRequest>> GetStudentRequestsAsync(string studentId);
        Task<List<MeetingRequest>> GetFacultyRequestsAsync(string facultyId);



    }
}
