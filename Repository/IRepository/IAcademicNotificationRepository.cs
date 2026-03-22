using ACRMS.Data;

namespace ACRMS.Repository.IRepository
{
    public interface IAcademicNotificationRepository
    {
        Task<List<AcademicNotification>> GetStudentNotificationsAsync(string studentId);
        Task<List<AcademicNotification>> GetFacultyNotificationsAsync(string facultyId);

        Task CreateAsync(AcademicNotification notification);
        Task MarkAsReadAsync(int id);
        Task MarkAllAsReadForStudentAsync(string studentId);
        Task MarkAllAsReadForFacultyAsync(string facultyId);

        Task<int> GetUnreadCountForStudentAsync(string studentId);
        Task<int> GetUnreadCountForFacultyAsync(string facultyId);
    }
}
