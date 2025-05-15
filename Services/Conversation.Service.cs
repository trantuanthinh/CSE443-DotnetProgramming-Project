using Microsoft.EntityFrameworkCore;
using Project.DTO;
using Project.Interfaces;
using Project.Models;
using Project.Repositories;

namespace Project.Services
{
    public class ConversationService(
        ConversationRepository repository,
        MessageRepository messageRepository
    ) : IConversationService
    {
        private readonly ConversationRepository _repository = repository;
        private readonly MessageRepository _messageRepository = messageRepository;

        #region Conversation

        public async Task<ICollection<Conversation>> GetConversations()
        {
            return await _repository.SelectAll().ToListAsync();
        }

        public async Task<Conversation> GetConversation(Guid id)
        {
            return await _repository.SelectAll().Where(item => item.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Conversation> FindOrCreateConversation(Guid senderId, Guid recipientId)
        {
            Conversation _item = await GetConversationByUserId(senderId, recipientId);

            if (_item == null)
            {
                _item = new Conversation
                {
                    Id = Guid.NewGuid(),
                    FirstUserId = senderId,
                    SecondUserId = recipientId,
                };
                await CreateConversation(_item);
            }
            return _item;
        }

        private async Task<Conversation> GetConversationByUserId(Guid senderId, Guid recipientId)
        {
            Conversation _item = await _repository
                .SelectAll()
                .Where(item =>
                    item.FirstUserId == senderId && item.SecondUserId == recipientId
                    || item.FirstUserId == recipientId && item.SecondUserId == senderId
                )
                .FirstOrDefaultAsync();
            return _item;
        }

        public async Task<bool> CreateConversation(Conversation _item)
        {
            _repository.Add(_item);
            return _repository.Save();
        }
        #endregion

        #region Message
        public async Task<List<Message>> GetMessagesByUserId(Guid senderId, Guid recipientId)
        {
            Conversation _item = await GetConversationByUserId(senderId, recipientId);
            if (_item == null)
            {
                return [];
            }
            return await GetMessagesByConversationId(_item.Id);
        }

        private async Task<List<Message>> GetMessagesByConversationId(Guid id)
        {
            List<Message> _items = await _messageRepository
                .SelectAll()
                .Where(message => message.ConversationId == id)
                .ToListAsync();
            return _items;
        }
        #endregion
    }
}
