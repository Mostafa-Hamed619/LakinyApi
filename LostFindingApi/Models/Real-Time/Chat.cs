using System;

namespace LostFindingApi.Models.Real_Time
{
    public class Chat
    {
        public int Id { get; set; }

        public string SenderId {  get; set; }

        public string ReceiverId { get; set; }

        public string Content { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public string File { get; set; } = string.Empty;

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}
