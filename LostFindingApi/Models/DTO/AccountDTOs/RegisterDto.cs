using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LostFindingApi.Models.DTO.AccountDTOs
{
    public class RegisterDto
    {
        public string UserName { get; set; }
        [Required]
        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$",ErrorMessage ="Invalid email address")]
        public string Email { get; set; }

        [Required]
        [StringLength(50,MinimumLength =6,ErrorMessage = "Password must be at least {2} and maximum {1} characters.")]
        public string Password { get; set; }

        [Required]
        [StringLength(15,ErrorMessage ="City must be at least{2} and maximum {1} character.")]
        public string City { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Region must be at least{2} and maximum {1} character.")]
        public string Region { get; set; }

         public IFormFile AccountPhoto { get; set; }

        [StringLength(11,ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }

        [Required]
        [RegularExpression("^([1-9]{1})([0-9]{2})([0-9]{2})([0-9]{2})([0-9]{2})[0-9]{3}([0-9]{1})[0-9]{1}$",ErrorMessage = "Invalid ID Card")]
        [StringLength(11,ErrorMessage = "Invalid ID card")]
        public string IdCard { get; set; }
    }
}
