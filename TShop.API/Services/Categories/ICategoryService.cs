using System.Linq.Expressions;
using TShop.API.Models;
using TShop.API.Services.IService;

namespace TShop.API.Services.Categories
{
    public interface ICategoryService : IService<Category> // Generic Interface
    {
        // ففش داعي IService الي عاملهم كومنتس هدول عشان عاملينهم في ال 

        // IEnumerable<Category> GetAllAsync(); // Collection(Array,List,HashSet,..) بورث منها ممكن يخزنها في subclass عشان اي
        // Category? GetAsync(Expression<Func<Category, bool>> predicate); // bool وبرجعلي (name,id,..) بصير ابعثله اي نوع بدي اياه سواء كان Controller عشان في ال
        // Task<Category> AddAsync(Category category, CancellationToken cancellationToken = default);
        Task<bool> EditAsync(int id, Category category, CancellationToken cancellationToken = default); // Update مش Edit بسموها Interface عادة في
        Task<bool> UpdateToggleAsync(int id, CancellationToken cancellationToken = default); // change Status property
        // Task<bool> RemoveAsync(int id, CancellationToken cancellationToken); // Delete مش Remove بسموها Interface عادة في
    }
}
