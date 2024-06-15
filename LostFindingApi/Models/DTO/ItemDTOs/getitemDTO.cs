using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LostFindingApi.Models.DTO.ItemDTOs
{
    public class getitemDTO
    {
        public int itemId { get; set; }
        public string itemStatus { get; set; }
        public string itemName { get;set; }
        public string description { get; set; }
        public string itemImage {  get; set; }
        public string itemAward {  get; set; }
        public string itemFoundPlace { get; set; }
        public string itemFoundDate {  get; set; }
        public string itemCategoryName { get; set; }
        public int itemCategoryId { get; set; }
        public string itemUserName { get; set; }
        public string itemUserEmail { get; set; }
        public string itemUserPhoto {  get; set; }
        public string itemUserPhone { get; set;}
        public double itemLatitude { get; set; }
        public double itemLongitude { get; set; }
    }
}
/*
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
*/