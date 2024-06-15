using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Collections.Generic;

namespace LostFindingApi.Models.Real_Time
{
    public class Notification
    {
        public int id {  get; set; }
        public string Title { get; set; }

        public string Content { get; set; }

        public List<string> DeviceIds { get; set; }
    }
}
