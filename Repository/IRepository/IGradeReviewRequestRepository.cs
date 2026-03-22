using ACRMS.Data;
using static ACRMS.Data.Enums;

namespace ACRMS.Repository.IRepository
{
    public interface IGradeReviewRequestRepository
    {
        Task<List<GradeReviewRequest>> GetStudentRequestsAsync(string studentId);
        Task<List<GradeReviewRequest>> GetFacultyRequestsAsync(string facultyId);

        Task<List<ApplicationUser>> GetFacultyAsync();
        Task<List<Course>> GetCoursesAsync();

        Task<string> GenerateReferenceNumberAsync();

        Task CreateAsync(GradeReviewRequest request);
        Task UpdateStatusAsync(int id, RequestStatus status, string? facultyNote);
        Task DeleteAsync(int id);
    }
}
