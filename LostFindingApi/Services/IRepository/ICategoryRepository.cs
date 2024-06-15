using LostFindingApi.Models.Data;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LostFindingApi.Services.IRepository
{
    public interface ICategoryRepository
    {
        public Task<IEnumerable<object>> getCategoryDetailsAsync();

        public Task<Category> AddCategoryAsync(Category category);

        public Task DeleteCategoryAsync(int id);

        public Task<IEnumerable<Category>> getCategoryName();

        public int GetCategoryId(string  categoryName);
    }
}
