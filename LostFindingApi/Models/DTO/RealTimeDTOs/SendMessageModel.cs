using Microsoft.AspNetCore.Http;

namespace LostFindingApi.Models.DTO.RealTimeDTOs
{
    public class SendMessageModel
    {
        public string ReceiverEmail { get; set; }
        public string Message { get; set; }
        public IFormFile File { get; set; }
        public double Latitude {  get; set; }
        public double Longitude { get; set; }
    }
}
