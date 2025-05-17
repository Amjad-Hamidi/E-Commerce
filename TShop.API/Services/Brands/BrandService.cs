using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TShop.API.Data;
using TShop.API.Models;
using TShop.API.Services.IService;

namespace TShop.API.Services.Brands
{
    public class BrandService : Service<Brand>, IBrandService // Generic Class
    {
        // connect to the database
        private readonly ApplicationDbContext dbContext;

        // ليمررها معه base لذلك نستخدم هون ال default مش رح يكون موجود ال parameterized constructor عشان موجود ال Service<Brand> هون مهمة كثير عشان لما يروح على base 
        public BrandService(ApplicationDbContext dbContext) : base(dbContext) 
        {
            this.dbContext = dbContext;
        }

        // Status بدنا نعدل كلشي باستثناء ال
        public async Task<bool> EditAsync(int id, Brand brand, CancellationToken cancellationToken)
        {
            Brand? brandInDb = dbContext.Brands.Find(id);                                    //Brand? brandInDb2 = dbContext.Brands.AsNoTracking().FirstOrDefault(b => b.Id == id);
            if (brandInDb == null)
                return false;

            // brandInDb.Id = id; // بكون يحملها ما في داعي يعني brandInDb لاحظ هون حذفتها لانو ما بلزم , ال 
            brandInDb.Name = brand.Name;
            brandInDb.Description = brand.Description;

            //dbContext.Brands.Update(brand); // Status لاحظ هاي حذفناها عشان ما في داعي لانا بدناش نعدل كلشي بما فيهن ال
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        // Status تعديل حالة ال 
        public async Task<bool> UpdateToggleAsync(int id, CancellationToken cancellationToken = default)
        {
            Category? categoryInDb = dbContext.Categories.Find(id);
            if (categoryInDb is null)
                return false;

            categoryInDb.Status = !categoryInDb.Status; // Toggle the status

            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

    }
}
