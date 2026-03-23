namespace ACRMS.Repository.IRepository
{
    public interface IDashboardRepository
    {
        Task<AdminDashboardDto> GetAdminDashboardAsync();
        Task<FacultyDashboardDto> GetFacultyDashboardAsync(string facultyId);
        Task<StudentDashboardDto> GetStudentDashboardAsync(string studentId);
    }

    public class AdminDashboardDto
    {
        public int UsersCount { get; set; }
        public int DepartmentsCount { get; set; }
        public int CoursesCount { get; set; }
        public int SectionsCount { get; set; }
        public int EnrollmentsCount { get; set; }
        public int AnnouncementsCount { get; set; }
    }

    public class FacultyDashboardDto
    {
        public int PendingMeetingRequestsCount { get; set; }
        public int PendingOfficialExcusesCount { get; set; }
        public int PendingGradeReviewRequestsCount { get; set; }
        public int UnreadNotificationsCount { get; set; }
        public int UnreadChatsCount { get; set; }
    }

    public class StudentDashboardDto
    {
        public int MeetingRequestsCount { get; set; }
        public int OfficialExcusesCount { get; set; }
        public int GradeReviewRequestsCount { get; set; }
        public int AnnouncementsCount { get; set; }
        public int UnreadNotificationsCount { get; set; }
        public int UnreadChatsCount { get; set; }
    }
}