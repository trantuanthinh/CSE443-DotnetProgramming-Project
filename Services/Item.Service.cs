using Microsoft.EntityFrameworkCore;
using Project.Interfaces;
using Project.Models;
using Project.Repositories;

namespace Project.Services
{
    public class ItemService(ItemRepository repository) : IItemService
    {
        private readonly ItemRepository _repository = repository;

        public async Task<ICollection<Item>> GetItems()
        {
            return await _repository.SelectAll().ToListAsync();
        }

        public async Task<Item> GetItem(Guid id)
        {
            return await _repository.SelectAll().Where(item => item.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> EditItem(Item item)
        {
            var existingItem = await GetItem(item.Id);
            if (existingItem == null)
            {
                return false;
            }

            existingItem.Name = item.Name;
            existingItem.Quantity = item.Quantity;
            existingItem.CategoryId = item.CategoryId;

            _repository.Update(existingItem);
            return await _repository.SaveAsync();
        }

        public async Task<bool> DeleteItem(Guid id)
        {
            _repository.Delete(id);
            return await _repository.SaveAsync();
        }
    }
}
