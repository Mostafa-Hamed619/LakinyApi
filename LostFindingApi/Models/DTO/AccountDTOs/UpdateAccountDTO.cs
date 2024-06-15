using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LostFindingApi.Models.DTO.AccountDTOs
{
    public class UpdateAccountDTO
    {
       
        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Phone { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public IFormFile AccountPhoto {  get; set; }
    }
}
