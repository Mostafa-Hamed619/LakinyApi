using LostFindingApi.Models;
using LostFindingApi.Models.Data;
using LostFindingApi.Models.DTO.ItemDTOs;
using LostFindingApi.Models.DTO.RealTimeDTOs;
using LostFindingApi.Services.IRepository;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Protocols;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace LostFindingApi.Services.Repository
{
    public class ItemMockRepository : ItemRepository
    {
        private readonly DataContext db;
        private readonly IFileRepository fileRepository;
        private readonly UserManager<Models.User> userManager;

        public ItemMockRepository(DataContext db,IFileRepository fileRepository,UserManager<Models.User> _userManager)
        {
            this.db = db;
            this.fileRepository = fileRepository;
            this.userManager = _userManager;
        }

        public async Task<Item> AddItemAsync(Item item)
        {
           await db.Item.AddAsync(item);
           await db.SaveChangesAsync();
            return item;
        }

        public IEnumerable<object> GetAllItemCategory()
        {
            var ItemCategory = db.Item
                .Join(
                    db.Category,
                    item => item.CategoryId,
                    category => category.Id,
                    (item, category) => new { c = category, i = item }
                    )
                .Join(
                    db.Users,
                    item => item.i.userId,
                    user => user.Id,
                     (item,user) => new { u = user , i = item}
                )

                .Select(ci => new {
                    itemId = ci.i.i.Id,
                    itemStatus = ci.i.i.Status,
                    itemImage = ci.i.i.ImagePhoto,
                    categoryName = ci.i.c.CategoryName,
                    itemName = ci.i.i.Name,
                    itemAward = ci.i.i.Award,
                    Description = ci.i.i.Description,
                    foundPlace = ci.i.i.FoundPlace,
                    foundDate = ci.i.i.FoundDate,
                    userId = ci.u.Id,
                    userName = ci.u.UserName,
                    Email = ci.u.Email,
                    UserImage = ci.u.AccountPhoto,
                    userPlace = ci.u.City,
                    phoneNumber = ci.u.PhoneNumber,
                    Latitude =ci.i.i.Latitude,
                    Longitude = ci.i.i.Longitude
                     }).OrderByDescending(item =>item.itemId);

            return ItemCategory;
        }


        public IEnumerable<Item> GetItems()
        {
            var result = db.Item.ToList();
            return result;
        }
        
        public void DeleteItemAsync(int id)
        {
            var result = db.Item.Find(id);

            db.Remove(result);
             db.SaveChanges();
        }

        public async Task<getitemDTO> getItem(int id)
        {
            var result = await db.Item
                .Join(
                db.Category,
                I => I.CategoryId,
                C => C.Id,
                (I, C) => new { Cate = C, Item = I }
                ).Join(
                db.Users,
                CI => CI.Item.userId,
                U => U.Id,
                (CI, U) => new { CateItem = CI, User = U }
                ).Select(d => new getitemDTO
                {
                    itemStatus = d.CateItem.Item.Status,
                    itemId = d.CateItem.Item.Id,
                    itemName = d.CateItem.Item.Name,
                    description = d.CateItem.Item.Description,
                    itemImage = d.CateItem.Item.ImagePhoto,
                    itemAward = d.CateItem.Item.Award,
                    itemFoundPlace = d.CateItem.Item.FoundPlace,
                    itemFoundDate = d.CateItem.Item.FoundDate,
                    itemCategoryName = d.CateItem.Cate.CategoryName,
                    itemCategoryId = d.CateItem.Cate.Id,
                    itemUserName = d.User.UserName,
                    itemUserEmail = d.User.Email,
                    itemUserPhoto = d.User.AccountPhoto,
                    itemUserPhone = d.User.PhoneNumber,
                    itemLatitude = d.CateItem.Item.Latitude,
                    itemLongitude = d.CateItem.Item.Longitude
                })
                .FirstOrDefaultAsync(i => i.itemId == id);
            return result;
        }

        public async Task<IEnumerable<object>> GetBySearch(string search)
        {
            if (search.Length >=3)
            {
                var result = db.Category
              .Join(
                  db.Item,
                  c => c.Id,
                  i => i.CategoryId,
                  (c, i) => new { Category = c, Item = i }
              )
              .Join(
                    db.Users,
                    i => i.Item.userId,
                    u => u.Id,
                    (i,u)=> new {CatItem = i , user = u}
                    )
              .Where(ci => ci.CatItem.Category.CategoryName.StartsWith(search) || ci.CatItem.Item.Name.Contains(search))
              .Select(ci => new {
                  itemStatus = ci.CatItem.Item.Status,
                  itemId = ci.CatItem.Item.Id,
                  itemName = ci.CatItem.Item.Name,
                  description = ci.CatItem.Item.Description,
                  itemImage = ci.CatItem.Item.ImagePhoto,
                  itemAward = ci.CatItem.Item.Award,
                  itemFoundPlace = ci.CatItem.Item.FoundPlace,
                  itemFoundDate = ci.CatItem.Item.FoundDate,
                  itemCategoryName = ci.CatItem.Category.CategoryName,
                  itemCategoryId = ci.CatItem.Category.Id,
                  itemUserName = ci.user.UserName,
                  itemUserEmail = ci.user.Email,
                  itemUserPhoto = ci.user.AccountPhoto,
                  itemUserPhone = ci.user.PhoneNumber,
              }).OrderByDescending(ci => ci.itemId);

                return result;
            }
            return null;
          
        }

        public async Task<IEnumerable<Item>> GetStatusFilter(string Status)
        {
            var result =await db.Item.Where(item => item.Status == Status).ToListAsync();
            return result;
        }

        public string getItemImage(int id)
        {
            var image = db.Item.FirstOrDefault(item => item.Id == id).ImagePhoto;
            return image;
        }

        public List<string> GetDescription()
        {
            return db.Item.Select(c => c.Description).ToList();
        }

        public void deleteUserItems(string id)
        {
            
            var result = db.Item
                .Join(
                db.Users,
                i => i.userId,
                u => u.Id,
                (i,u) => new {item = i,user = u}
                ).Select(join => join.item).ToList();

            var UserItem = result.Where(u => u.userId == id).ToList();

            foreach (var item in UserItem)
            {
                // Delete associated image using FileRepository
                if (!string.IsNullOrEmpty(item.ImagePhoto))
                {
                    fileRepository.DeleteImage(item.ImagePhoto);
                }

                db.Item.Remove(item);
            }
            db.SaveChanges();
        }

        public async Task<IEnumerable<getUserItemDTO>> getUserItems(string id)
        {
           
            var user = await userManager.FindByIdAsync(id);

            var items = db.Item.Where(user => user.userId == id)
                .Select(ui => new getUserItemDTO
                {
                    itemImage = ui.ImagePhoto,
                    itemStatus = ui.Status,
                    itemName = ui.Name,
                    itemAward = ui.Award,
                    Description = ui.Description,
                    foundPlace = ui.FoundPlace,
                    foundDate = ui.FoundDate,
                    Latitude = ui.Latitude,
                    Longitude = ui.Longitude
                })
                .ToList();

            return items;
        }

        public async Task AddCard(Card card)
        {
            await db.cards.AddAsync(card);
            await db.SaveChangesAsync();
        }

        public getLastItemDTO LastPosted()
        {
            var result = db.Item
                     .OrderByDescending(it => it.Id)
                     .Select(it => new getLastItemDTO
                     {
                         Id = it.Id,
                         ItemName = it.Name,
                         ItemPhoto = it.ImagePhoto,
                         Status = it.Status  
                     })
                     .FirstOrDefault();

            return result;
        }
    }
}
