using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _repository.BeginTransactionAsync();
        }

        public async Task<ICollection<BorrowTransaction>> GetItems()
        {
            return await _repository
                .SelectAll()
                .Include(item => item.Item)
                .Include(item => item.Borrower)
                .ToListAsync();
        }

        public async Task<BorrowTransaction> GetItem(Guid id)
        {
            return await _repository
                .SelectAll()
                .Include(item => item.Item)
                .Where(item => item.Id == id)
                .FirstOrDefaultAsync();
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

        public string GenerateBorrowResponseBody(
            string username,
            int quantity,
            string status,
            DateTime pickupTime
        )
        {
            return $@"
    <!DOCTYPE html>
    <html lang=""en"">
    <head>
        <meta charset=""UTF-8"">
        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
        <title>Borrow Request Response</title>
    </head>
    <body style=""font-family: Arial, sans-serif; background-color: #f4f4f9; margin: 0; padding: 20px;"">
        <div style=""background-color: #ffffff; padding: 20px 30px; max-width: 500px; border-radius: 8px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1); text-align: center; margin: 0 auto;"">
            <h2 style=""color: #333;"">Borrow Request Status</h2>
            <p style=""font-size: 16px; color: #444;"">
                Hello <strong>{username}</strong>,
            </p>
            <p style=""font-size: 16px; color: #555; margin: 16px 0;"">
                Your borrow request for <strong>{quantity}</strong> item(s) has been 
                <strong style=""color: {(status == "Approved" ? "#28a745" : "#dc3545")};"">{status}</strong>.
            </p>
            <p style=""font-size: 16px; color: #444;"">
                <strong>Pickup Date:</strong> {pickupTime:dd/MM/yyyy}
            </p>
            <p style=""font-size: 14px; color: #888; margin-top: 20px;"">
                Thank you for using our service.
            </p>
        </div>
    </body>
    </html>";
        }

        public string GenerateOverdueBody(string username, int quantity, DateTime? dueDate)
        {
            return $@"
    <!DOCTYPE html>
    <html lang=""en"">
    <head>
        <meta charset=""UTF-8"">
        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
        <title>Overdue Notification</title>
    </head>
    <body style=""font-family: Arial, sans-serif; background-color: #f4f4f9; margin: 0; padding: 20px;"">
        <div style=""background-color: #ffffff; padding: 20px 30px; max-width: 500px; border-radius: 8px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1); text-align: center; margin: 0 auto;"">
            <h2 style=""color: #333;"">Overdue Notice</h2>
            <p style=""font-size: 16px; color: #444;"">
                Hello <strong>{username}</strong>,
            </p>
            <p style=""font-size: 16px; color: #555; margin: 16px 0;"">
                This is a reminder that <strong>{quantity}</strong> item(s) you borrowed are now overdue. The due date was <strong>{(dueDate.HasValue ? dueDate.Value.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy"))}</strong>.
            </p>
            <p style=""font-size: 14px; color: #888; margin-top: 20px;"">
                Please return the items as soon as possible. Thank you for your attention.
            </p>
        </div>
    </body>
    </html>";
        }
    }
}
