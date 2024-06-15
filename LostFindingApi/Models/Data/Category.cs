using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LostFindingApi.Models.Data
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string CategoryName { get; set; }

    }
}
