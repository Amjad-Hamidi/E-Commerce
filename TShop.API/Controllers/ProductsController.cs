using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TShop.API.Data;
using TShop.API.DTOs.Products.Requests;
using TShop.API.DTOs.Products.Responses;
using TShop.API.Models;
using TShop.API.Services.Products;
using TShop.API.Utility;

namespace TShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{StaticData.SuperAdmin}, {StaticData.Admin}, {StaticData.Company}")]
    public class ProductsController(IProductService productService) : ControllerBase
    {
        private readonly IProductService productService = productService;

        [HttpGet("")]
        [AllowAnonymous] // AllowAnonymous: لا يحتاج الى تسجيل دخول
        public IActionResult GelAllProducts([FromQuery] string? query, [FromQuery] int page, [FromQuery] int limit = 10)
        {
            // Pagination using IQueryable
            // response time اعمل فلتر وهاد بقلل من ال DB اني بقدر مباشرة من السيرفر وقبل لاوصل ال IQueryable فائدة ال
            // DB بل انا بفلترها ثم بستدعي ال RAM وال DB ما بتتحمل على IQueryable يعني ال 
            IQueryable<Product> products = productService.GetProducts();
            if (string.IsNullOrEmpty(query) != true) // Or: query != null
            {                       // query بناء عالاسم او الوصف بصير يفلتر اذا دخل قيمة لل
                products = products.Where(p => p.Name.Contains(query) || p.Description.Contains(query));
            }
            /*
              Console.WriteLine($"page = {page} ... query = {query}"); // page = 0 , query = ""
             */
            if(page <= 0 && limit <= 0) // page و limit فحص اذا ال
            {
                page = 1; // بلش من اول صفحة
                limit = 10; // بلش من حد 10
            }
            else if (page <= 0) // page فحص اذا ال
            {
                page = 1; // بلش من اول صفحة
            }
            else if (limit <= 0) // limit فحص اذا ال
            {
                limit = 10; // بلش من حد 10
            }

            products = products
                .Skip((page - 1) * limit) // Skip the first (page - 1) * limit products
                .Take(limit); // Take the next limit products

            // Adapt is a method from Mapster that maps the Product object to ProductResponse object
            return Ok(products.Adapt<IEnumerable<ProductResponse>>());
        }

        [HttpGet("{id}")]
        [AllowAnonymous] // AllowAnonymous: لا يحتاج الى تسجيل دخول
        public IActionResult GetProductById([FromRoute] int id)
        {
            var product = productService.Get(p => p.Id == id);
                                                // DTO : auto-mapping with Mapster
            return product == null ? NotFound() : Ok(product.Adapt<ProductResponse>());
        }

        [HttpPost("")]                   // FromForm is used to get the file from the form
        public IActionResult CreateProduct([FromForm] ProductRequest productRequest)
        {
            var productInDb = productService.Add(productRequest.Adapt<Product>(), productRequest.MainImg);

            if(productInDb is not null)
            {
                return CreatedAtAction(nameof(GetProductById), new { id = productInDb.Id }, productInDb);
            }
            return BadRequest();


            /* // without using the ProductService
            var file = productRequest.MainImg;
            // string هو Product وفي IFormFile هو ProductRequest في MainImg لانو نوع ال Adapt لازم اعمل
            var product = productRequest.Adapt<Product>(); 

            if (file is not null && file.Length > 0) // فحص اذا الفايل مبعوث وايضا التاكد انه مش فاضي
            {
                // Guid.NewGuid() creates a new GUID (Globally Unique Identifier) => هاد الرقم بكون فريد من نوعه مستحيل اذا عملت اكثر من واحد يجيب نفس الرقم
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); // Path.GetExtension(file.FileName) gets the extension of the file
                
                // Path.Combine is used to combine the path of the current directory with the path of the images folder and the file name
                // images وضع الملف الي رفعنا في مجلد ال
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "images", fileName); // Directory.GetCurrentDirectory() gets the current directory of the project => TShop.API
                // لحد الان ما حفظنا الصورة في قاعدة البيانات بس حفظناها في مجلد الصور

                // الان رح نربط الفايل اللي جبناه من الفورم بالمسار اللي حفظنا فيه الصورة
                using (var stream = System.IO.File.Create(filePath)) // File.Create(filePath) creates a new file in the images folder
                {
                    file.CopyTo(stream); // بايت بايت بنسخ images نسخ الملف الي رفعه اليوزر ووضعه في الفايل الي انحفظ داخل مجلد ال
                }

                // DB التخزين في
                product.MainImg = fileName; // DB تخزين اسم الملف في  
                _context.Products.Add(product);
                _context.SaveChanges();

                // CreatedAtAction returns a 201 status code with the product object in the response body
                return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
            }

            return BadRequest();
            */
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct([FromRoute] int id, [FromForm] ProductUpdateRequest productUpdateRequest)
        {
            var product = productService.Edit(id, productUpdateRequest.Adapt<Product>(), productUpdateRequest.MainImg);
            if (!product)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct([FromRoute] int id)
        {
            var product = productService.Remove(id);
            if (!product)
            {
                return NotFound();
            }
            return NoContent();


            /*
            var product = _context.Products.Find(id);
            if (product is null)
            {
                return NotFound();
            }

            // Delete the image from the images folder
            // images بحذف ايضا الملف من مجلد ال DB رح اخزن ملفات عالفاضي يعني مساحة اكبر عالفاضي لما احذف من ال images بدون ما احذف الفايل الموجود في مجلد ال DB ت تحديدا احنا منخزن اسم الملف , فاذا بس بحذف من Product يعني في DB لانو في DB لازم احذف الصورة من المجلد قبل ما احذفها من  
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "images", product.MainImg);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // DB حذف المنتج من
            _context.Products.Remove(product);
            _context.SaveChanges();
            return NoContent();
            */
        }
    }
}
