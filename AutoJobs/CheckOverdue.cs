using Project.Interfaces;
using Project.MailServices;
using Project.Models;
using Project.Utils;
using Quartz;

namespace Project.AutoJobs
{
    public class CheckOverDue(IBorrowTransactionService service, MailService mailService) : IJob
    {
        private readonly IBorrowTransactionService _service = service;
        private readonly MailService _mailService = mailService;

        public async Task Execute(IJobExecutionContext context)
        {
            ICollection<BorrowTransaction> items = await _service.GetItems();
            if (items == null)
            {
                return;
            }
            foreach (var item in items)
            {
                if (item.Status == ItemStatus.Borrowing && item.DueDate < DateTime.Now)
                {
                    Console.WriteLine($"CheckOverDue! Time: {DateTime.Now} - {item.DueDate}");
                    item.Status = ItemStatus.Overdue;
                    await _service.EditItem(item);
                    string body = _service.GenerateOverdueBody(
                        item.Borrower.Name,
                        item.Quantity,
                        DateTime.Now
                    );
                    if (
                        !await _mailService.SendMail(
                            item.Borrower.Email,
                            "Borrow Overdue Status",
                            body
                        )
                    )
                    {
                        throw new Exception("Failed to send email");
                    }
                }
            }
            Console.WriteLine("CheckOverDue!");
            await Task.CompletedTask;
        }
    }
}
