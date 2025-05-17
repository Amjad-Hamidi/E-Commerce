using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TShop.API.Data;
using TShop.API.DTOs.Products.Requests;
using TShop.API.Models;

namespace TShop.API.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext dbContext;

        public ProductService(ApplicationDbContext dbContext) 
        {
            this.dbContext = dbContext;
        }

        public Product Add(Product product, IFormFile mainFile)
        {
            var file = mainFile;
           
            if (file is not null && file.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "images", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                }

                product.MainImg = fileName;
            }

            dbContext.Products.Add(product);
            dbContext.SaveChanges();
            return product;
        }

        public bool Edit(int id, Product product, IFormFile mainFile)
        {
            Product? productInDb = dbContext.Products.AsNoTracking().FirstOrDefault(x => x.Id == id);
            if (productInDb == null) return false;

            product.Id = id;

            var file = mainFile;
            if (file is not null && file.Length > 0) // بغيرها Edit Request في حال كان مدخل صورة في ال 
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "images", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                }

                // Delete the old image file from the images folder
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "images", productInDb.MainImg);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                // Save the new image file name in the database
                product.MainImg = fileName;
            }
            else
            {   // if the user didn't upload a new image, keep the old one
                product.MainImg = productInDb.MainImg;
            }

            dbContext.Products.Update(product);
            dbContext.SaveChanges();
            return true;
        }

        public Product? Get(Expression<Func<Product, bool>> expression)
        {
            return dbContext.Products.FirstOrDefault(expression);
        }

        public IQueryable<Product> GetProducts() // ProductsController في GelAllProducts في Pagination بتفيدنا بال IQueryable
        {
            IQueryable<Product> products = dbContext.Products;
            
            return products;
        }

        public bool Remove(int id)
        {
            Product? productInDb = dbContext.Products.Find(id);
            if(productInDb == null) return false;

            // Delete the file from the images folder
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "images", productInDb.MainImg);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Delete the product from the database
            dbContext.Products.Remove(productInDb);
            dbContext.SaveChanges();
            return true;
        }
    }
}
