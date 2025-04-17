using Project.Models;

namespace Project.Interfaces
{
    public interface IBorrowTransactionService
    {
        Task<ICollection<BorrowTransaction>> GetItems();
        Task<BorrowTransaction> GetItem(Guid id);
        Task<bool> CreateItem(BorrowTransaction item);
        Task<bool> EditItem(BorrowTransaction item);
        Task<bool> DeleteItem(Guid id);
    }
}
