using Project.Interfaces;
using Project.Models;
using Project.Repositories;

namespace Project.Services
{
    // DONE
    public class MessageService(MessageRepository repository) : IMessageService
    {
        private readonly MessageRepository _repository = repository;

        public async Task<bool> CreateMessage(Message _item)
        {
            _item.Timestamp = DateTime.UtcNow;
            _repository.Add(_item);
            return _repository.Save();
        }

        public async Task<bool> DeleteMessage(Guid id)
        {
            _repository.Delete(id);
            return _repository.Save();
        }
    }
}
