using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TShop.API.Data;
using TShop.API.DTOs.Categories.Requests;
using TShop.API.DTOs.Categories.Responses;
using TShop.API.Models;
using TShop.API.Services.Categories;
using TShop.API.Utility;

namespace TShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = $"{StaticData.SuperAdmin}, {StaticData.Admin}, {StaticData.Company}")] // Roles لازم يكون مسجل دخول وكذلك يحمل احدى هذه ال
    [Authorize(Roles = $"{StaticData.SuperAdmin}")] // NotFound() بعدها يقدر , بدونها برجعله Login وهاي بتكون من خلال ال cookie ممنوع حدا يفوت عليه الا اذا حد معه
    public class CategoriesController(ICategoryService categoryService) : ControllerBase
    {
        private readonly ICategoryService categoryService = categoryService;


        [HttpGet("")]
        [AllowAnonymous] // AllowAnonymous: لا يحتاج الى تسجيل دخول
        public async Task<IActionResult> getAll()
        {
            var categories = await categoryService.GetAsync();
            
            return Ok(categories.Adapt<IEnumerable<CategoryResponse>>());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getById([FromRoute]int id)
        {
            Category? category = await categoryService.GetOneAsync(e => e.Id == id);
                                                
            return category ==null? NotFound() : Ok(category.Adapt<CategoryResponse>());
        }

        [HttpPost("")]
        [Authorize(Roles = $"{StaticData.SuperAdmin}, {StaticData.Admin}, {StaticData.Company}")]
        public async Task<IActionResult> Create([FromBody] CategoryRequest categoryRequset, CancellationToken cancellationToken)
        {          
            Category categoryInDb = await categoryService.AddAsync(categoryRequset.Adapt<Category>(), cancellationToken); // DB في حالة صار في خلل عند الاضافة في request عشان نقدر نوقف ال : CancellationToken 

            return CreatedAtAction(nameof(getById),new {categoryInDb.Id}, categoryInDb); // CategoryResponse بس بدون ما يحول الى status code 201 هيك بعمللي ال 
        }


        [HttpPut("{id}")]
        [Authorize(Roles = $"{StaticData.SuperAdmin}, {StaticData.Admin}, {StaticData.Company}")]
        public async Task<IActionResult> Update([FromRoute]int id,[FromBody] CategoryRequest categoryRequset)
        {
            var categoryInDb = await categoryService.EditAsync(id, categoryRequset.Adapt<Category>());
            if (!categoryInDb) return NotFound();
            return NoContent(); 
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{StaticData.SuperAdmin}, {StaticData.Admin}, {StaticData.Company}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            var categoryInDb = await categoryService.RemoveAsync(id, cancellationToken);
            if (!categoryInDb) return NotFound();
            return NoContent(); 
        }



    }
}
