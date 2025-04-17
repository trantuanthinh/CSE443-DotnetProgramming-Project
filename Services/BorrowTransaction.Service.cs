using Microsoft.EntityFrameworkCore;
using Project.AppContext;
using Project.Interfaces;
using Project.Models;
using Project.Repositories;

namespace Project.Services
{
    public class BorrowTransactionService(BorrowTransactionRepository repository)
        : IBorrowTransactionService
    {
        private readonly BorrowTransactionRepository _repository = repository;

        public async Task<ICollection<BorrowTransaction>> GetItems()
        {
            return await _repository.SelectAll().ToListAsync();
        }

        public async Task<BorrowTransaction> GetItem(Guid id)
        {
            return await _repository.SelectAll().Where(item => item.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> CreateItem(BorrowTransaction item)
        {
            _repository.Add(item);
            return await _repository.SaveAsync();
        }

        //todo
        public async Task<bool> EditItem(BorrowTransaction item)
        {
            var existingItem = await GetItem(item.Id);
            if (existingItem == null)
            {
                return false;
            }

            existingItem.Status = item.Status;
            existingItem.Quantity = item.Quantity;

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
