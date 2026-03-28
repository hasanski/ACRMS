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
            var conversations = await _context.Conversations
                .AsNoTracking()
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Include(c => c.Messages)
                    .ThenInclude(m => m.SenderUser)
                .Include(c => c.Section)
                    .ThenInclude(s => s.Course)
                .Where(c => c.IsActive && c.Participants.Any(p => p.UserId == userId))
                .ToListAsync();

            var filtered = new List<Conversation>();

            foreach (var conv in conversations)
            {
                if (conv.Type != ConversationType.Section)
                {
                    filtered.Add(conv);
                    continue;
                }

                var section = await _context.Sections
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == conv.SectionId);

                if (section is null)
                    continue;

                if (section.FacultyMemberId == userId)
                {
                    filtered.Add(conv);
                    continue;
                }

                if (conv.IsVisibleToStudents)
                {
                    filtered.Add(conv);
                }
            }

            return filtered
                .OrderByDescending(c => c.Messages.Any()
                    ? c.Messages.Max(m => m.SentAt)
                    : c.CreatedAt)
                .ToList();
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
            var canSend = await CanUserSendToConversationAsync(conversationId, senderUserId);
            if (!canSend)
                throw new Exception("غير مسموح لك بإرسال رسالة في هذه المحادثة");

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
        public async Task<List<string>> GetConversationParticipantIdsAsync(int conversationId)
        {
            return await _context.ConversationParticipants
                .AsNoTracking()
                .Where(p => p.ConversationId == conversationId)
                .Select(p => p.UserId)
                .ToListAsync();
        }

        public async Task<Dictionary<int, int>> GetUnreadCountsForUserAsync(string userId)
        {
            var conversationIds = await _context.ConversationParticipants
                .Where(cp => cp.UserId == userId)
                .Select(cp => cp.ConversationId)
                .ToListAsync();

            var counts = await _context.Messages
                .Where(m => conversationIds.Contains(m.ConversationId) &&
                            !m.IsDeleted &&
                            m.SenderUserId != userId)
                .GroupJoin(
                    _context.MessageReads.Where(r => r.UserId == userId),
                    m => m.Id,
                    r => r.MessageId,
                    (m, reads) => new { m.ConversationId, IsRead = reads.Any() }
                )
                .Where(x => !x.IsRead)
                .GroupBy(x => x.ConversationId)
                .Select(g => new
                {
                    ConversationId = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return counts.ToDictionary(x => x.ConversationId, x => x.Count);
        }
        public async Task<Conversation?> GetConversationByIdAsync(int conversationId)
        {
            return await _context.Conversations
                .Include(c => c.Section)
                    .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(c => c.Id == conversationId);
        }

        public async Task UpdateSectionConversationSettingsAsync(int conversationId, bool isVisibleToStudents, bool studentsCanReply)
        {
            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.Id == conversationId && c.Type == ConversationType.Section);

            if (conversation is null)
                return;

            conversation.IsVisibleToStudents = isVisibleToStudents;
            conversation.StudentsCanReply = studentsCanReply;

            await _context.SaveChangesAsync();
        }
        public async Task<List<Conversation>> GetSectionConversationsForUserAsync(string userId)
        {
            var conversations = await _context.Conversations
                .AsNoTracking()
                .Include(c => c.Section)
                    .ThenInclude(s => s.Course)
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Include(c => c.Messages)
                    .ThenInclude(m => m.SenderUser)
                .Where(c => c.IsActive &&
                            c.Type == ConversationType.Section &&
                            c.Participants.Any(p => p.UserId == userId))
                .ToListAsync();

            return conversations
                .OrderByDescending(c => c.Messages.Any()
                    ? c.Messages.Max(m => m.SentAt)
                    : c.CreatedAt)
                .ToList();
        }

        public async Task<int> CreateSectionConversationIfNotExistsAsync(int sectionId, string facultyId)
        {
            var existing = await _context.Conversations
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Type == ConversationType.Section &&
                                          c.SectionId == sectionId &&
                                          c.IsActive);

            if (existing is not null)
            {
                await SyncSectionConversationParticipantsAsync(existing.Id, sectionId, facultyId);
                return existing.Id;
            }

            var section = await _context.Sections
                .Include(s => s.Course)
                .FirstAsync(s => s.Id == sectionId);

            var conversation = new Conversation
            {
                Title = $"شعبة {section.SectionName} - {section.Course?.CourseName}",
                Type = ConversationType.Section,
                SectionId = sectionId,
                CreatedByUserId = facultyId,
                IsActive = true,
                IsReadOnly = false,
                IsVisibleToStudents = true,
                StudentsCanReply = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();

            var participants = new List<ConversationParticipant>();

            participants.Add(new ConversationParticipant
            {
                ConversationId = conversation.Id,
                UserId = facultyId
            });

            var studentIds = await _context.Enrollments
                .Where(e => e.SectionId == sectionId)
                .Select(e => e.StudentId)
                .Distinct()
                .ToListAsync();

            foreach (var studentId in studentIds)
            {
                if (!participants.Any(p => p.UserId == studentId))
                {
                    participants.Add(new ConversationParticipant
                    {
                        ConversationId = conversation.Id,
                        UserId = studentId
                    });
                }
            }

            _context.ConversationParticipants.AddRange(participants);
            await _context.SaveChangesAsync();

            return conversation.Id;
        }

        public async Task<bool> CanUserSendToConversationAsync(int conversationId, string userId)
        {
            var conversation = await _context.Conversations
                .Include(c => c.Section)
                .FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversation is null)
                return false;

            if (conversation.Type == ConversationType.Private)
                return true;

            if (conversation.Type == ConversationType.Section)
            {
                var section = await _context.Sections
                    .FirstOrDefaultAsync(s => s.Id == conversation.SectionId);

                if (section is null)
                    return false;

                var isFaculty = section.FacultyMemberId == userId;
                if (isFaculty)
                    return true;

                var isStudentParticipant = await _context.ConversationParticipants
                    .AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId);

                if (!isStudentParticipant)
                    return false;

                if (!conversation.IsVisibleToStudents)
                    return false;

                return conversation.StudentsCanReply;
            }

            return true;
        }

        public async Task SyncSectionConversationParticipantsAsync(int conversationId, int sectionId, string facultyId)
        {
            var existingParticipantIds = await _context.ConversationParticipants
                .Where(p => p.ConversationId == conversationId)
                .Select(p => p.UserId)
                .ToListAsync();

            var targetStudentIds = await _context.Enrollments
                .Where(e => e.SectionId == sectionId)
                .Select(e => e.StudentId)
                .Distinct()
                .ToListAsync();

            var requiredIds = new List<string> { facultyId };
            requiredIds.AddRange(targetStudentIds);

            var missingIds = requiredIds
                .Where(id => !existingParticipantIds.Contains(id))
                .Distinct()
                .ToList();

            if (missingIds.Any())
            {
                var newParticipants = missingIds.Select(id => new ConversationParticipant
                {
                    ConversationId = conversationId,
                    UserId = id
                }).ToList();

                _context.ConversationParticipants.AddRange(newParticipants);
                await _context.SaveChangesAsync();
            }
        }
    }
}