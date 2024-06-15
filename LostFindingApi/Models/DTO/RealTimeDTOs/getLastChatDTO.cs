using System;

namespace LostFindingApi.Models.DTO.RealTimeDTOs
{
    public class getLastChatDTO
    {
        public int id {  get; set; }
        public string content {  get; set; }
        public DateTime Time { get; set; }
    }
}
