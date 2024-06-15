using Microsoft.Identity.Client;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LostFindingApi.Models.Data
{
    public class Item
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20,ErrorMessage = "Item name must be at maximum {1} characters.")]
        public string Name { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 20, ErrorMessage = "Item Description must be at least {2} and maximum {1} characters.")]
        public string Description { get; set; }

        [Required]
        public string Status { get; set; }

        public string Award { get; set; }

        public string? ImagePhoto { get;set; }

        public string FoundDate { get; set; }

        [Required]
        public string FoundPlace { get;set; }

        public int CategoryId { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string userId { get; set; }

        public double similarity_score { get; set; }

    }
}
