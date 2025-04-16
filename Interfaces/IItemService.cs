using Project.Models;

namespace Project.Interfaces
{
    public interface IItemService
    {
        Task<ICollection<Item>> GetItems(User user);
        Task<Item> GetItem(Guid id);
        Task<bool> EditItem(Item item);
        Task<bool> DeleteItem(Guid id);
    }
}
