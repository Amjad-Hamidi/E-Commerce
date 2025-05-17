using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TShop.API.Data;
using TShop.API.Models;
using TShop.API.Services.IService;

namespace TShop.API.Services.Categories
{
    public class CategoryService : Service<Category>, ICategoryService // Generic Class
    {
        private readonly ApplicationDbContext _context;

        // ليمررها معه base لذلك نستخدم هون ال default مش رح يكون موجود ال parameterized constructor عشان موجود ال Service<Category> هون مهمة كثير عشان لما يروح على base
        public CategoryService(ApplicationDbContext context) : base(context) // DbContext الي هو Service لل _context بمرر ال 
        {
            this._context = context;
        }


        // Status بدنا نعدل كلشي باستثناء ال
        public async Task<bool> EditAsync(int id, Category category, CancellationToken cancellationToken = default)
        {
            Category? categoryInDb = _context.Categories.Find(id);// OR : Category? categoryInDb2 = _context.Categories.AsNoTracking().FirstOrDefault(b => b.Id == id);
            
            if (categoryInDb is null)
                return false;
            // categoryInDb.Id = id; // بكون يحملها ما في داعي يعني categoryInDb لاحظ هون حذفتها لانو ما بلزم , ال 
            categoryInDb.Name = category.Name;
            categoryInDb.Description = category.Description;
            //  _context.Categories.Update(category); // Status لاحظ هاي حذفناها عشان ما في داعي لانا بدناش نعدل كلشي بما فيهن ال
            await _context.SaveChangesAsync(cancellationToken); 
            return true;
        }

        // Status تعديل حالة ال 
        public async Task<bool> UpdateToggleAsync(int id, CancellationToken cancellationToken = default)
        {
            Category? categoryInDb = _context.Categories.Find(id);
            if (categoryInDb is null)
                return false;

            categoryInDb.Status = !categoryInDb.Status; // Toggle the status

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
