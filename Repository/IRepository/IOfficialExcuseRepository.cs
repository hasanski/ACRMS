using ACRMS.Data;
using static ACRMS.Data.Enums;

namespace ACRMS.Repository.IRepository
{
    public interface IOfficialExcuseRepository
    {
        Task<List<OfficialExcuse>> GetStudentExcusesAsync(string studentId);
        Task<List<OfficialExcuse>> GetFacultyExcusesAsync(string facultyId);

        Task<List<ApplicationUser>> GetFacultyAsync();

        Task<string> GenerateReferenceNumberAsync();

        Task CreateAsync(OfficialExcuse officialExcuse);
        Task UpdateStatusAsync(int id, RequestStatus status, string? reviewerNote);
        Task DeleteAsync(int id);
    }
}
