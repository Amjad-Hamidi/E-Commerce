using System.Linq.Expressions;
using TShop.API.Models;
using TShop.API.Services.IService;

namespace TShop.API.Services.Brands
{
    public interface IBrandService : IService<Brand> // Generic Interface
    {
        // ففش داعي IService الي عاملهم كومنتس هدول عشان عاملينهم في ال 

        //IEnumerable<Brand> GetBrands();
        //Brand? Get(Expression<Func<Brand, bool>> predicate);
        //Brand Add(Brand brand);
        Task<bool> EditAsync(int id, Brand brand, CancellationToken cancellationToken = default);
        Task<bool> UpdateToggleAsync(int id, CancellationToken cancellationToken = default);
        //bool Remove(int id);
    }
}
