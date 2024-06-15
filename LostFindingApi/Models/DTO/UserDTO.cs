using System.ComponentModel.DataAnnotations;

namespace LostFindingApi.Models.DTO
{
    public class UserDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }

        public string Userid { get; set; }

        public string PhoneNumber { get; set; }

        public string City { get; set; }

        public string JWT { get; set; }

        public string Title { get; set; }

        public string Message { get;set; }

        public bool status { get; set; }
    }
}
