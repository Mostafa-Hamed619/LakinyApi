using LostFindingApi.Models.Data;
using LostFindingApi.Services.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostFindingApi.Services.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext db;

        public CategoryRepository(DataContext db)
        {
            this.db = db;
        }

        public async Task<Category> AddCategoryAsync(Category category)
        {
            var NewCategory = new Category()
            {
                CategoryName = category.CategoryName,
            };
            await db.AddAsync(NewCategory);
            await db.SaveChangesAsync();
            return NewCategory;
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var Category =await db.Category.FirstOrDefaultAsync(c => c.Id == id);
            db.Category.Remove(Category);
            await db.SaveChangesAsync();
        }

        public async Task<IEnumerable<object>> getCategoryDetailsAsync()
        {
            var result = await db.Category
                .Join(
                    db.Item,
                    category => category.Id,
                    item => item.CategoryId,
                    (category, item) => new { c = category, i = item }
                    )
                .Join(
                    db.Users,
                    ci => ci.i.userId,
                    user => user.Id,
                    (ci, user) => new
                    {
                        categoryName = ci.c.CategoryName, ItemName = ci.i.Name, ItemPhoto = ci.i.ImagePhoto, ItemPlace = ci.i.FoundPlace,
                        ItemDate = ci.i.FoundDate, userName = user.UserName, Email = user.Email
                    }
                ).ToListAsync();
               
              
            return result;
        }

        public int GetCategoryId(string categoryName)
        {
            var result = db.Category.FirstOrDefault(category => category.CategoryName == categoryName).Id;

            return result;
        }

        public async Task<IEnumerable<Category>> getCategoryName()
        {
            return await db.Category.ToListAsync();
        }
    }
}
