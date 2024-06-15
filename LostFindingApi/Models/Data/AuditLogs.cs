using System;

namespace LostFindingApi.Models.Data
{
    public class AuditLogs
    {
        public Guid Id { get; set; }
        public string User { get; set; }
        public required string EntityType { get; set; }
        public required string Action { get; set; }
        public required DateTime TimeStamp { get; set; }
        public required string Changes { get; set; }
    }
}
