using System.Linq.Expressions;

namespace TShop.API.Services.IService
{
    public interface IService <T> where T : class // Generic Interface
    {
        Task<IEnumerable<T>> GetAsync(Expression<Func<T,bool>> ?expression = null, // ممكن اكون في شرط معين بدي اياه ارجع المنتجات/الاصناف/الماركة بناء عليه
            Expression<Func<T, object>>?[] includes = null, // [] (array) اكثر من علاقة للجدول الواحد لذلك قد احتاج لفكرة ال
            bool isTracked = true); 
        Task<T?> GetOneAsync(Expression<Func<T, bool>> predicate, // (عشان ممكن درجع معلومات جدول ثاني في هاد الجدول (اختيارية includes ضفت ال 
            Expression<Func<T,object>>? []includes = null,
            bool isTracked = true);
        Task<T> AddAsync(T category,
            CancellationToken cancellationToken = default);
        Task<bool> RemoveAsync(int id,
            CancellationToken cancellationToken);

        Task<int> CommitAsync(CancellationToken cancellationToken = default);
    }
}
