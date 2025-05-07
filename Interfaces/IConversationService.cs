using Project.DTO;
using Project.Models;

namespace Project.Interfaces
{
    public interface IConversationService
    {
        Task<ICollection<Conversation>> GetConversations();
        Task<Conversation> GetConversation(Guid id);

        Task<Conversation> FindOrCreateConversation(Guid senderId, Guid recipientId);
        Task<List<MessageResponse>> GetMessagesByUserId(Guid senderId, Guid recipientId);
    }
}
