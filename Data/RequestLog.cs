using System.ComponentModel.DataAnnotations;

namespace ACRMS.Data
{
    public class RequestLog : BaseEntity
    {
        [Required, MaxLength(100)]
        public string EntityName { get; set; } = string.Empty;

        public int EntityId { get; set; }

        [Required, MaxLength(100)]
        public string ActionType { get; set; } = string.Empty;

        [Required]
        public string PerformedByUserId { get; set; } = string.Empty;
        public ApplicationUser? PerformedByUser { get; set; }

        public string? Notes { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
