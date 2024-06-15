using System.ComponentModel.DataAnnotations;

namespace LostFindingApi.Models.DTO.AccountDTOs
{
    public class ResetPasswordDTO
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage = "Invalid email address")]
        public string Email {  get; set; }

        [Required]
        public string newPassword { get; set; }
    }
}
