using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using static System.Collections.Specialized.BitVector32;

namespace ACRMS.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string UniversityNumber { get; set; } = string.Empty;

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Course> TaughtCourses { get; set; } = new List<Course>();
        public ICollection<Section> TaughtSections { get; set; } = new List<Section>();

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();


        public ICollection<Announcement> AnnouncementsCreated { get; set; } = new List<Announcement>();
        public ICollection<AnnouncementRecipient> AnnouncementRecipients { get; set; } = new List<AnnouncementRecipient>();

        public ICollection<AcademicNotification> NotificationsSent { get; set; } = new List<AcademicNotification>();
        public ICollection<AcademicNotification> NotificationsReceived { get; set; } = new List<AcademicNotification>();

        public ICollection<MeetingRequest> MeetingRequestsAsStudent { get; set; } = new List<MeetingRequest>();
        public ICollection<MeetingRequest> MeetingRequestsAsFaculty { get; set; } = new List<MeetingRequest>();

        public ICollection<Appointment> AppointmentsAsStudent { get; set; } = new List<Appointment>();
        public ICollection<Appointment> AppointmentsAsFaculty { get; set; } = new List<Appointment>();

        public ICollection<OfficialExcuse> OfficialExcusesAsStudent { get; set; } = new List<OfficialExcuse>();
        public ICollection<OfficialExcuse> OfficialExcusesAsFaculty { get; set; } = new List<OfficialExcuse>();

        public ICollection<GradeReviewRequest> GradeReviewRequestsAsStudent { get; set; } = new List<GradeReviewRequest>();
        public ICollection<GradeReviewRequest> GradeReviewRequestsAsFaculty { get; set; } = new List<GradeReviewRequest>();

        public ICollection<Inquiry> InquiriesAsStudent { get; set; } = new List<Inquiry>();
        public ICollection<Inquiry> InquiriesAsFaculty { get; set; } = new List<Inquiry>();

        public ICollection<AcademicNote> AcademicNotesWritten { get; set; } = new List<AcademicNote>();
        public ICollection<AcademicNote> AcademicNotesReceived { get; set; } = new List<AcademicNote>();

        public ICollection<Conversation> ConversationsCreated { get; set; } = new List<Conversation>();
        public ICollection<ConversationParticipant> ConversationParticipants { get; set; } = new List<ConversationParticipant>();
        public ICollection<Message> MessagesSent { get; set; } = new List<Message>();
        public ICollection<MessageRead> MessageReads { get; set; } = new List<MessageRead>();

        public ICollection<RequestLog> RequestLogs { get; set; } = new List<RequestLog>();

    }

}
