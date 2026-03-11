using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ACRMS.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Section> Sections => Set<Section>();
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();

        public DbSet<Announcement> Announcements => Set<Announcement>();
        public DbSet<AnnouncementRecipient> AnnouncementRecipients => Set<AnnouncementRecipient>();
        public DbSet<AcademicNotification> AcademicNotifications => Set<AcademicNotification>();

        public DbSet<MeetingRequest> MeetingRequests => Set<MeetingRequest>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<OfficialExcuse> OfficialExcuses => Set<OfficialExcuse>();
        public DbSet<GradeReviewRequest> GradeReviewRequests => Set<GradeReviewRequest>();
        public DbSet<Inquiry> Inquiries => Set<Inquiry>();
        public DbSet<AcademicNote> AcademicNotes => Set<AcademicNote>();

        public DbSet<Conversation> Conversations => Set<Conversation>();
        public DbSet<ConversationParticipant> ConversationParticipants => Set<ConversationParticipant>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<MessageRead> MessageReads => Set<MessageRead>();

        public DbSet<RequestLog> RequestLogs => Set<RequestLog>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Conversation>()
    .HasOne(c => c.CreatedByUser)
    .WithMany(u => u.ConversationsCreated)
    .HasForeignKey(c => c.CreatedByUserId)
    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(m => m.SenderUser)
                .WithMany(u => u.MessagesSent)
                .HasForeignKey(m => m.SenderUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<ConversationParticipant>()
    .HasOne(cp => cp.User)
    .WithMany(u => u.ConversationParticipants)
    .HasForeignKey(cp => cp.UserId)
    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ConversationParticipant>()
                .HasOne(cp => cp.Conversation)
                .WithMany(c => c.Participants)
                .HasForeignKey(cp => cp.ConversationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ConversationParticipant>()
                .HasOne(cp => cp.LastReadMessage)
                .WithMany()
                .HasForeignKey(cp => cp.LastReadMessageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MessageRead>()
                .HasOne(mr => mr.User)
                .WithMany(u => u.MessageReads)
                .HasForeignKey(mr => mr.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MessageRead>()
                .HasOne(mr => mr.Message)
                .WithMany(m => m.Reads)
                .HasForeignKey(mr => mr.MessageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<RequestLog>()
                .HasOne(r => r.PerformedByUser)
                .WithMany(u => u.RequestLogs)
                .HasForeignKey(r => r.PerformedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Announcement>()
                 .HasOne(a => a.Faculty)
                 .WithMany(u => u.AnnouncementsCreated)
                 .HasForeignKey(a => a.FacultyId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Announcement>()
                .HasOne(a => a.TargetStudent)
                .WithMany()
                .HasForeignKey(a => a.TargetStudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AnnouncementRecipient>()
                .HasOne(ar => ar.Student)
                .WithMany(u => u.AnnouncementRecipients)
                .HasForeignKey(ar => ar.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AcademicNotification>()
                .HasOne(n => n.Faculty)
                .WithMany(u => u.NotificationsSent)
                .HasForeignKey(n => n.FacultyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AcademicNotification>()
                .HasOne(n => n.Student)
                .WithMany(u => u.NotificationsReceived)
                .HasForeignKey(n => n.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MeetingRequest>()
                .HasOne(m => m.Student)
                .WithMany(u => u.MeetingRequestsAsStudent)
                .HasForeignKey(m => m.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MeetingRequest>()
                .HasOne(m => m.Faculty)
                .WithMany(u => u.MeetingRequestsAsFaculty)
                .HasForeignKey(m => m.FacultyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(a => a.Student)
                .WithMany(u => u.AppointmentsAsStudent)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(a => a.Faculty)
                .WithMany(u => u.AppointmentsAsFaculty)
                .HasForeignKey(a => a.FacultyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<OfficialExcuse>()
                .HasOne(o => o.Student)
                .WithMany(u => u.OfficialExcusesAsStudent)
                .HasForeignKey(o => o.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<OfficialExcuse>()
                .HasOne(o => o.Faculty)
                .WithMany(u => u.OfficialExcusesAsFaculty)
                .HasForeignKey(o => o.FacultyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<GradeReviewRequest>()
                .HasOne(g => g.Student)
                .WithMany(u => u.GradeReviewRequestsAsStudent)
                .HasForeignKey(g => g.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<GradeReviewRequest>()
                .HasOne(g => g.Faculty)
                .WithMany(u => u.GradeReviewRequestsAsFaculty)
                .HasForeignKey(g => g.FacultyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<GradeReviewRequest>()
                .HasOne(g => g.Course)
                .WithMany(c => c.GradeReviewRequests)
                .HasForeignKey(g => g.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<GradeReviewRequest>()
                .Property(g => g.CurrentMark)
                .HasPrecision(5, 2);

            builder.Entity<Inquiry>()
                .HasOne(i => i.Student)
                .WithMany(u => u.InquiriesAsStudent)
                .HasForeignKey(i => i.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Inquiry>()
                .HasOne(i => i.Faculty)
                .WithMany(u => u.InquiriesAsFaculty)
                .HasForeignKey(i => i.FacultyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AcademicNote>()
                .HasOne(n => n.Faculty)
                .WithMany(u => u.AcademicNotesWritten)
                .HasForeignKey(n => n.FacultyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AcademicNote>()
                .HasOne(n => n.Student)
                .WithMany(u => u.AcademicNotesReceived)
                .HasForeignKey(n => n.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Course>()
                .HasOne(c => c.FacultyMember)
                .WithMany(u => u.TaughtCourses)
                .HasForeignKey(c => c.FacultyMemberId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Section>()
                .HasOne(s => s.FacultyMember)
                .WithMany(u => u.TaughtSections)
                .HasForeignKey(s => s.FacultyMemberId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Course>()
                .HasIndex(c => c.CourseCode)
                .IsUnique();

            builder.Entity<MeetingRequest>()
                .HasIndex(m => m.ReferenceNumber)
                .IsUnique();

            builder.Entity<OfficialExcuse>()
                .HasIndex(o => o.ReferenceNumber)
                .IsUnique();

            builder.Entity<GradeReviewRequest>()
                .HasIndex(g => g.ReferenceNumber)
                .IsUnique();

            builder.Entity<Inquiry>()
                .HasIndex(i => i.ReferenceNumber)
                .IsUnique();
        }
    }
}