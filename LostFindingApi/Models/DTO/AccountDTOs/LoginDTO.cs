using System.ComponentModel.DataAnnotations;

namespace LostFindingApi.Models.DTO.AccountDTOs
{
    public class LoginDTO
    {
        [Required,EmailAddress]
        public string Email { get; set; }

        [Required,DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
