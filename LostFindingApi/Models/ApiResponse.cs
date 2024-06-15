using System.Collections.Generic;

namespace LostFindingApi.Models
{
    public class ApiResponse
    {
        public string Title { get; set; }

        public string Message { get; set; }

        public bool Status { get; set; }

        public List<string> Errors { get; set; }

        public int HttpStatusCode { get; set; }

        public string Descriptions { get; set; }
    }
}
