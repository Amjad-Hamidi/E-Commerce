using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TShop.API.Data;
using TShop.API.Models;

namespace TShop.API.Services.IService
{
    public class Service<T> : IService<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Service(ApplicationDbContext context)
        {
            this._context = context;
            this._dbSet = context.Set<T>(); // من حاله Entity عشان يعرف ال DbSet حقن ال 
        }


        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _context.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        // بتحبلي مجموعة شغلات بناء عشرط معين اذا كان مبعوث وداتا الجدول الثانية المربوطة بالجدول الحالي اذا كان مبعوث
        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? expression = null,Expression<Func<T, object>>?[] includes = null,
            bool isTracked = true)
        {
            IQueryable<T> entities = _dbSet; // IQueryable عشني استخدمت resources بجيب الداتا من السيرفر مباشرة وهاد بقلل 
           
            if(expression != null) // معناها في شرط مبعوث , مثلا: بدي المنتجات الي سعرهم اكثر من 50 دولار
            {
                entities = entities.Where(expression); // هيك بجيب الداتا بناء على الشرط الي هو باعثه
            }

            if (includes != null) // معناها في علاقة بين الجداول
            {
                foreach (var include in includes)
                {
                    entities = entities.Include(include); // هيك بجيب الداتا تاعت العلاقة مع الجداول الاخرى اذا كانت موجودة
                }
            }

            if (!isTracked) // معناها انو ما بدي اتابع التغييرات الي بتصير على الداتا
            {
                entities = entities.AsNoTracking(); // هيك بوقف التتبع
            }

            return await entities.ToListAsync(); // Async بشكل  List عشان احولها ل : ToListAsync()

        }

        public async Task<T?> GetOneAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>?[] includes = null,
            bool isTracked = true)
        {
            var all = await GetAsync(predicate, includes, isTracked); // بجيبلي كل الداتا بناء على الشرط الي بعثته

            return all.FirstOrDefault(); // List بجيبلي اول عنصر من ال 


            //return _dbSet.FirstOrDefault(predicate); // Or: _context.Set<T>().FirstOrDefault(predicate);
        }

        public async Task<bool> RemoveAsync(int id, CancellationToken cancellationToken = default)
        {
            T? entityInDb = _dbSet.Find(id); // ? : means Nullable
            if (entityInDb == null) return false;

            _dbSet.Remove(entityInDb);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
           return await _context.SaveChangesAsync(cancellationToken);
        }


    }
}
