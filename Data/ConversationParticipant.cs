namespace ACRMS.Data
{
    public class ConversationParticipant : BaseEntity
    {
        public int ConversationId { get; set; }
        public Conversation? Conversation { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public int? LastReadMessageId { get; set; }
        public Message? LastReadMessage { get; set; }

        public bool IsMuted { get; set; } = false;
    }
}
