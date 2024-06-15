namespace LostFindingApi.Models.DTO.ItemDTOs
{
    public class getUserItemDTO
    {
        public string itemId {get; set;}
        public string itemStatus {get; set;}
        public string itemImage { get; set; }
        public string categoryName { get; set; }
        public string itemName { get; set; }
        public string itemAward { get; set; }
        public string Description { get; set; }
        public string foundPlace { get; set; }
        public string foundDate  { get; set;}
        public string userId { get; set; }
        public string userName { get; set; }
        public string Email { get; set; }
        public string UserImage { get; set; }
        public string userPlace { get; set; }
        public string phoneNumber { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
