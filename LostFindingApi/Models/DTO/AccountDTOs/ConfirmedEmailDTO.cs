using System.ComponentModel.DataAnnotations;

namespace LostFindingApi.Models.DTO.AccountDTOs
{
    public class ConfirmedEmailDTO
    {
        [Required]
    
        public string username { get; set; }

        [Required]
        public string code { get; set; }
    }
}
