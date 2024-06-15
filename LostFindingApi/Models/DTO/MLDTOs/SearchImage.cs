using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LostFindingApi.Models.DTO.MLDTOs
{
    public class SearchImage
    {
        [Required]
        public IFormFile Image { get; set; }
    }
}
