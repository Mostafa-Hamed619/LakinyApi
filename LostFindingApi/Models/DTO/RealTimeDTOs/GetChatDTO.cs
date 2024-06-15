using System;

namespace LostFindingApi.Models.DTO.RealTimeDTOs
{
    public class GetChatDTO
    {
        public string senderEmail { get; set; }
        public string receiverEmail { get; set; }
        public string content { get; set; }
        public int id { get; set; }
        public DateTime Time { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
