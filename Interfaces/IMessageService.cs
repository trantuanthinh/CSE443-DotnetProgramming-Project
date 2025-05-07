using Project.Models;

namespace Project.Interfaces
{
    public interface IMessageService
    {
        Task<bool> CreateMessage(Message _item);
        Task<bool> DeleteMessage(Guid id);
    }
}
