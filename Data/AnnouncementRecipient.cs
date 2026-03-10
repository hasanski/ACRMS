namespace ACRMS.Data
{
    public class AnnouncementRecipient : BaseEntity
    {
        public int AnnouncementId { get; set; }
        public Announcement? Announcement { get; set; }

        public string StudentId { get; set; } = string.Empty;
        public ApplicationUser? Student { get; set; }

        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }
    }
}
