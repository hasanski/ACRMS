namespace ACRMS.Data
{
    public class Enums
    {
        //public enum AnnouncementTargetType
        //{
        //    AllStudents = 1,
        //    Course = 2,
        //    Section = 3,
        //    SingleStudent = 4
        //}
        public enum AnnouncementTargetType
        {
            AllStudents = 1,
            ByDepartment = 2,
            ByCourse = 3,
            BySection = 4,
            IndividualStudent = 5
        }

        public enum AnnouncementPriority
        {
            Low = 1,
            Normal = 2,
            High = 3,
            Urgent = 4
        }

        public enum NotificationType
        {
            General = 1,
            Warning = 2,
            Reminder = 3,
            Summon = 4
        }

        public enum RequestStatus
        {
            Pending = 1,
            Approved = 2,
            Rejected = 3,
            Completed = 4,
            Rescheduled = 5,
            Archived = 6
        }

        public enum MeetingType
        {
            InPerson = 1,
            Online = 2,
            PhoneCall = 3
        }

        public enum InquiryStatus
        {
            Open = 1,
            Replied = 2,
            Closed = 3
        }

        public enum NoteVisibility
        {
            StudentAndFaculty = 1,
            FacultyOnly = 2,
            AdminOnly = 3
        }

        public enum ConversationType
        {
            Private = 1,
            Group = 2
        }
    }
}
