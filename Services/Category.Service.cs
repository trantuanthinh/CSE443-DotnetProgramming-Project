using Microsoft.EntityFrameworkCore;
using Project.Interfaces;
using Project.Models;
using Project.Repositories;

namespace Project.Services
{
    public class CategoryService(CategoryRepository repository) : ICategoryService
    {
        private readonly CategoryRepository _repository = repository;

        public async Task<ICollection<Category>> GetItems()
        {
            return await _repository.SelectAll().ToListAsync();
        }

        public async Task<Category> GetItem(Guid id)
        {
            return await _repository.SelectAll().Where(item => item.Id == id).FirstOrDefaultAsync();
        }
    }
}
