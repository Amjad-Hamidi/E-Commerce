using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TShop.API.DTOs.Brands.Requests;
using TShop.API.DTOs.Brands.Responses;
using TShop.API.Models;
using TShop.API.Services.Brands;

namespace TShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController(IBrandService brandService) : ControllerBase
    {
        private readonly IBrandService brandService = brandService;


        [HttpGet("")]
        public async Task<IActionResult> getAll()
        {
            IEnumerable<Brand> brands = await brandService.GetAsync();

            return Ok(brands.Adapt<IEnumerable<BrandResponse>>());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getById(int id)
        {
            Brand? brand = await brandService.GetOneAsync(e => e.Id == id);

            return brand == null ? NotFound() : Ok(brand.Adapt<BrandResponse>());
        }

        [HttpPost("")]
        public async Task<IActionResult> Create([FromBody] BrandRequest brandRequest, CancellationToken cancellationToken)
        {
            Brand? brandInDb = await brandService.AddAsync(brandRequest.Adapt<Brand>(), cancellationToken); // DB في حالة صار في خلل عند الاضافة في request عشان نقدر نوقف ال : CancellationToken 

            return CreatedAtAction(nameof(getById), new { brandInDb.Id }, brandInDb);  // BrandResponse بس بدون ما يحول الى status code 201 هيك بعمللي ال 
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BrandRequest brandRequest)
        {
            bool brandInDb = await brandService.EditAsync(id, brandRequest.Adapt<Brand>());

            if (!brandInDb)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            bool brandInDb = await brandService.RemoveAsync(id, cancellationToken);

            if (!brandInDb)
                return NotFound();

            return NoContent();
        }


    }
}