using Project.Models;

namespace Project.Interfaces
{
    public interface IItemService
    {
        Task<ICollection<Item>> GetItems();
        Task<Item> GetItem(Guid id);
        Task<bool> CreateItem(Item item);
        Task<bool> EditItem(Item item);
        Task<bool> DeleteItem(Guid id);
    }
}
