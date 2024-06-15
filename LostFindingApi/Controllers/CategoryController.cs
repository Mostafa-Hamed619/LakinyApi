using LostFindingApi.Models.Data;
using LostFindingApi.Models.DTO.CategoryDTOs;
using LostFindingApi.Services.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LostFindingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles ="Admin")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository categoryRepository;


        public CategoryController(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        [HttpGet("GetAllDetails")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllDetails()
        {
            var result = await categoryRepository.getCategoryDetailsAsync();
            return Ok(result);
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll()
        {
            var result = await categoryRepository.getCategoryName();
            return Ok(result);
        }

        [HttpPost("Add-category")]
        public async Task<ActionResult<Category>> AddCategory([FromBody]AddCategoryDTO newCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Category Name is Required");
            }
            var cateory = new Category { CategoryName = newCategory.CategoryName };
            var result = await categoryRepository.AddCategoryAsync(cateory);
            return Ok(result);
        }

        [HttpDelete("Delete-category/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await categoryRepository.DeleteCategoryAsync(id);
            return Ok("Delete process successfully done.");
        }
    }
}
