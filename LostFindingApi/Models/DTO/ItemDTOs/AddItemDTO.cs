using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System;

namespace LostFindingApi.Models.DTO.ItemDTOs
{
    public class AddItemDTO
    {
        [Required]
        [MaxLength(20, ErrorMessage = "Item name must be at maximum {1} characters.")]
        public string Name { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Item Description must be at least {2} and maximum {1} characters.")]
        public string Description { get; set; }

        [Required]
        public string Status { get; set; }

        public string Award { get; set; }

        
        [Required]
        public IFormFile? ImagePhoto { get; set; }

        public string FoundDate { get; set; }
        [Required]
        public string FoundPlace { get; set; }

        public string CategoryName { get; set; }

        public double Latitude { get; set; } = 0;

        public double Longitude { get; set; } = 0;

        public string userId { get; set; }
    }
}
