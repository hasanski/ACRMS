using ACRMS.Data;
using ACRMS.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using static ACRMS.Data.Enums;

namespace ACRMS.Repository
{
    public class ChatRepository: IChatRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Conversation>> GetUserConversationsAsync(string userId)
        {
            return await _context.Conversations
                .AsNoTracking()
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Include(c => c.Messages)
                    .ThenInclude(m => m.SenderUser)
                .Where(c => c.IsActive && c.Participants.Any(p => p.UserId == userId))
                .OrderByDescending(c => c.Messages.Max(m => (DateTime?)m.SentAt) ?? c.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Message>> GetConversationMessagesAsync(int conversationId)
        {
            return await _context.Messages
                .AsNoTracking()
                .Include(m => m.SenderUser)
                .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<List<ApplicationUser>> GetAvailableUsersForChatAsync(string currentUserId)
        {
            return await _context.Users
                .AsNoTracking()
                .Include(u => u.Department)
                .Where(u => u.Id != currentUserId && u.IsActive)
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<int?> FindPrivateConversationAsync(string user1Id, string user2Id)
        {
            var conversation = await _context.Conversations
                .AsNoTracking()
                .Include(c => c.Participants)
                .Where(c => c.Type == ConversationType.Private && c.IsActive)
                .FirstOrDefaultAsync(c =>
                    c.Participants.Count == 2 &&
                    c.Participants.Any(p => p.UserId == user1Id) &&
                    c.Participants.Any(p => p.UserId == user2Id));

            return conversation?.Id;
        }

        public async Task<int> CreatePrivateConversationAsync(string currentUserId, string otherUserId)
        {
            var existingId = await FindPrivateConversationAsync(currentUserId, otherUserId);
            if (existingId.HasValue)
                return existingId.Value;

            var currentUser = await _context.Users.FirstAsync(x => x.Id == currentUserId);
            var otherUser = await _context.Users.FirstAsync(x => x.Id == otherUserId);

            var conversation = new Conversation
            {
                Title = $"{currentUser.FullName} - {otherUser.FullName}",
                Type = ConversationType.Private,
                CreatedByUserId = currentUserId,
                IsActive = true,
                IsReadOnly = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();

            _context.ConversationParticipants.AddRange(
                new ConversationParticipant
                {
                    ConversationId = conversation.Id,
                    UserId = currentUserId
                },
                new ConversationParticipant
                {
                    ConversationId = conversation.Id,
                    UserId = otherUserId
                }
            );

            await _context.SaveChangesAsync();

            return conversation.Id;
        }

        public async Task SendMessageAsync(int conversationId, string senderUserId, string messageText)
        {
            var msg = new Message
            {
                ConversationId = conversationId,
                SenderUserId = senderUserId,
                MessageText = messageText.Trim(),
                SentAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();
        }

        public async Task MarkConversationAsReadAsync(int conversationId, string userId)
        {
            var lastMessage = await _context.Messages
                .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
                .OrderByDescending(m => m.SentAt)
                .FirstOrDefaultAsync();

            if (lastMessage is null)
                return;

            var participant = await _context.ConversationParticipants
                .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

            if (participant is not null)
            {
                participant.LastReadMessageId = lastMessage.Id;
            }

            var unreadMessages = await _context.Messages
                .Where(m => m.ConversationId == conversationId &&
                            !m.IsDeleted &&
                            m.SenderUserId != userId)
                .ToListAsync();

            foreach (var msg in unreadMessages)
            {
                var alreadyRead = await _context.MessageReads
                    .AnyAsync(x => x.MessageId == msg.Id && x.UserId == userId);

                if (!alreadyRead)
                {
                    _context.MessageReads.Add(new MessageRead
                    {
                        MessageId = msg.Id,
                        UserId = userId,
                        ReadAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUnreadCountAsync(int conversationId, string userId)
        {
            var messageIds = await _context.Messages
                .Where(m => m.ConversationId == conversationId &&
                            !m.IsDeleted &&
                            m.SenderUserId != userId)
                .Select(m => m.Id)
                .ToListAsync();

            var readIds = await _context.MessageReads
                .Where(r => r.UserId == userId && messageIds.Contains(r.MessageId))
                .Select(r => r.MessageId)
                .ToListAsync();

            return messageIds.Count(id => !readIds.Contains(id));
        }
    }
}