namespace LostFindingApi.Models.DTO
{
    public class EmailSendDTO
    {

        public EmailSendDTO(string to, string subject, string body)
        {
            Body = body;
            To = to;
            Subject = subject;
        }
        public string To { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}
