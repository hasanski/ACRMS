using ACRMS.Data;

namespace ACRMS.Repository.IRepository
{
    public interface IChatRepository
    {
        Task<List<Conversation>> GetUserConversationsAsync(string userId);
        Task<List<Message>> GetConversationMessagesAsync(int conversationId);
        Task<List<ApplicationUser>> GetAvailableUsersForChatAsync(string currentUserId);

        Task<int> CreatePrivateConversationAsync(string currentUserId, string otherUserId);
        Task<int?> FindPrivateConversationAsync(string user1Id, string user2Id);

        Task SendMessageAsync(int conversationId, string senderUserId, string messageText);
        Task MarkConversationAsReadAsync(int conversationId, string userId);

        Task<int> GetUnreadCountAsync(int conversationId, string userId);
    }
}
