using System.ComponentModel.DataAnnotations;

namespace ACRMS.Data
{
    public class Message : BaseEntity
    {
        public int ConversationId { get; set; }
        public Conversation? Conversation { get; set; }

        [Required]
        public string SenderUserId { get; set; } = string.Empty;
        public ApplicationUser? SenderUser { get; set; }

        [Required]
        public string MessageText { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? AttachmentPath { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;

        public ICollection<MessageRead> Reads { get; set; } = new List<MessageRead>();
    }
}
