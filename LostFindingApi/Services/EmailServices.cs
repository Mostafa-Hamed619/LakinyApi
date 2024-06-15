using LostFindingApi.Models.DTO;
using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Transactions;

namespace LostFindingApi.Services
{
    public class EmailServices
    {
        private readonly IConfiguration _config;

        public EmailServices(IConfiguration config)
        {
            this._config = config;
        }

        public async Task<bool> SendEmailAsync(EmailSendDTO emailSendDTO)
        {
           
            MailjetClient Client = new MailjetClient(_config["MailJet:ApiKey"], _config["MailJet:SecretKey"]);

            var From = _config["Email:From"];
            var ApplicationName = _config["Email:ApplicationName"];


            var email = new TransactionalEmailBuilder().
                WithFrom(new SendContact(From, ApplicationName))
                .WithSubject(emailSendDTO.Subject)
                .WithHtmlPart(emailSendDTO.Body)
                .WithTo(new SendContact(emailSendDTO.To))
                .Build();

            var response = await Client.SendTransactionalEmailAsync(email);

            if(response.Messages != null)
            {
                if (response.Messages[0].Status == "success")
                {
                    return true;
                }
            }
            return false;
        }
    }
}
