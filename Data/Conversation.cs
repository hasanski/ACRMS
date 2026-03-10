using System.ComponentModel.DataAnnotations;
using static ACRMS.Data.Enums;

namespace ACRMS.Data
{
    public class Conversation : BaseEntity
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public ConversationType Type { get; set; } = ConversationType.Private;

        [Required]
        public string CreatedByUserId { get; set; } = string.Empty;
        public ApplicationUser? CreatedByUser { get; set; }

        public int? CourseId { get; set; }
        public Course? Course { get; set; }

        public int? SectionId { get; set; }
        public Section? Section { get; set; }

        public string? Description { get; set; }

        public bool IsReadOnly { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
