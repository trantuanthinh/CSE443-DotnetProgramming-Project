using Project.Models;

namespace Project.Interfaces
{
    public interface ICategoryService
    {
        Task<ICollection<Category>> GetItems();
        Task<Category> GetItem(Guid id);
        // Task<bool> CreateItem(Item item);
        // Task<bool> EditItem(Item item);
        // Task<bool> DeleteItem(Guid id);
    }
}
