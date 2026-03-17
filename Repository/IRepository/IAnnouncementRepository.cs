using ACRMS.Data;
using static ACRMS.Data.Enums;

namespace ACRMS.Repository.IRepository
{
    public interface IAnnouncementRepository
    {
        Task<List<Announcement>> ListAsync();

        Task<List<Department>> GetDepartmentsAsync();
        Task<List<Course>> GetCoursesAsync();
        Task<List<Section>> GetSectionsAsync();
        Task<List<ApplicationUser>> GetStudentsAsync();
        Task<List<ApplicationUser>> GetFacultyAsync();

        Task CreateAsync(Announcement announcement);
        Task DeleteAsync(int id);

        Task<List<ApplicationUser>> ResolveRecipientsAsync(
            AnnouncementTargetType targetType,
            int? departmentId,
            int? courseId,
            int? sectionId,
            string? targetStudentId);

        Task CreateRecipientsAsync(int announcementId, List<string> studentIds);
    }
}
