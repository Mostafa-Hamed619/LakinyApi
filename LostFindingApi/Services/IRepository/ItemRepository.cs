using LostFindingApi.Models.Data;
using LostFindingApi.Models.DTO.ItemDTOs;
using LostFindingApi.Models.DTO.RealTimeDTOs;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LostFindingApi.Services.IRepository
{
    public interface ItemRepository
    {
        public Task<Item> AddItemAsync(Item item);

        public IEnumerable<object> GetAllItemCategory();

        public IEnumerable<Item> GetItems();

        public Task<getitemDTO> getItem(int id);

        public string getItemImage(int id);

        public void DeleteItemAsync(int id);

        public Task<IEnumerable<object>> GetBySearch(string search);

        public Task<IEnumerable<Item>> GetStatusFilter(string Status);

        public List<string> GetDescription();

        public void deleteUserItems(string id);

        public Task<IEnumerable<getUserItemDTO>> getUserItems(string id);

        public Task AddCard(Card card);

        public getLastItemDTO LastPosted();
    }
}
