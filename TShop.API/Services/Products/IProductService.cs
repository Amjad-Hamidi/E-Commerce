using System.Linq.Expressions;
using TShop.API.Models;

namespace TShop.API.Services.Products
{
    public interface IProductService
    {
        IQueryable<Product> GetProducts(); // ProductsController في GelAllProducts في Pagination بتفيدنا بال IQueryable
        Product? Get(Expression<Func<Product, bool>> expression);
        Product Add(Product product, IFormFile file); // IFormFile is used to get the file from the form
        bool Edit(int id, Product product, IFormFile file); // IFormFile is used to get the file from the form
        bool Remove(int id);
    }
}
