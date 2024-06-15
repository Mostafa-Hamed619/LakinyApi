using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace LostFindingApi.Models
{
    public class User : IdentityUser
    {
        [MaxLength(20)]
        public string idCard { get; set; }

        [Required, MaxLength(15)]
        public string City { get; set; }

        
        public string region { get; set; }

        public string AccountPhoto { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    }
}
