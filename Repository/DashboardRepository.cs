using ACRMS.Data;
using ACRMS.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using static ACRMS.Data.Enums;

namespace ACRMS.Repository
{
    public class DashboardRepository: IDashboardRepository
    {
        private readonly ApplicationDbContext _context;

        public DashboardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardDto> GetAdminDashboardAsync()
        {
            return new AdminDashboardDto
            {
                UsersCount = await _context.Users.CountAsync(),
                DepartmentsCount = await _context.Departments.CountAsync(),
                CoursesCount = await _context.Courses.CountAsync(),
                SectionsCount = await _context.Sections.CountAsync(),
                EnrollmentsCount = await _context.Enrollments.CountAsync(),
                AnnouncementsCount = await _context.Announcements.CountAsync()
            };
        }

        public async Task<FacultyDashboardDto> GetFacultyDashboardAsync(string facultyId)
        {
            var unreadChatCount = await _context.ConversationParticipants
                .Include(cp => cp.Conversation)
                .Include(cp => cp.LastReadMessage)
                .Where(cp => cp.UserId == facultyId && cp.Conversation != null && cp.Conversation.IsActive)
                .CountAsync(cp =>
                    cp.Conversation!.Messages.Any(m =>
                        !m.IsDeleted &&
                        m.SenderUserId != facultyId &&
                        (cp.LastReadMessageId == null || m.Id > cp.LastReadMessageId)));

            return new FacultyDashboardDto
            {
                PendingMeetingRequestsCount = await _context.MeetingRequests
                    .CountAsync(x => x.FacultyId == facultyId && x.Status == RequestStatus.Pending),

                PendingOfficialExcusesCount = await _context.OfficialExcuses
                    .CountAsync(x => x.FacultyId == facultyId && x.Status == RequestStatus.Pending),

                PendingGradeReviewRequestsCount = await _context.GradeReviewRequests
                    .CountAsync(x => x.FacultyId == facultyId && x.Status == RequestStatus.Pending),

                UnreadNotificationsCount = await _context.AcademicNotifications
                    .CountAsync(x => x.FacultyId == facultyId && !x.IsRead),

                UnreadChatsCount = unreadChatCount
            };
        }

        public async Task<StudentDashboardDto> GetStudentDashboardAsync(string studentId)
        {
            var unreadChatCount = await _context.ConversationParticipants
                .Include(cp => cp.Conversation)
                .Include(cp => cp.LastReadMessage)
                .Where(cp => cp.UserId == studentId && cp.Conversation != null && cp.Conversation.IsActive)
                .CountAsync(cp =>
                    cp.Conversation!.Messages.Any(m =>
                        !m.IsDeleted &&
                        m.SenderUserId != studentId &&
                        (cp.LastReadMessageId == null || m.Id > cp.LastReadMessageId)));

            return new StudentDashboardDto
            {
                MeetingRequestsCount = await _context.MeetingRequests
                    .CountAsync(x => x.StudentId == studentId),

                OfficialExcusesCount = await _context.OfficialExcuses
                    .CountAsync(x => x.StudentId == studentId),

                GradeReviewRequestsCount = await _context.GradeReviewRequests
                    .CountAsync(x => x.StudentId == studentId),

                AnnouncementsCount = await _context.AnnouncementRecipients
                    .CountAsync(x => x.StudentId == studentId),

                UnreadNotificationsCount = await _context.AcademicNotifications
                    .CountAsync(x => x.StudentId == studentId && !x.IsRead),

                UnreadChatsCount = unreadChatCount
            };
        }
    }
}