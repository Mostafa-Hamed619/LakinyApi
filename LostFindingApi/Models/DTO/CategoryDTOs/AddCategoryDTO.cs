using System.ComponentModel.DataAnnotations;

namespace LostFindingApi.Models.DTO.CategoryDTOs
{
    public class AddCategoryDTO
    {
        [Required]
        public string CategoryName { get; set; }
    }
}
