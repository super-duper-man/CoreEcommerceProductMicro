
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using Resource.Share.Lib.Logs;
using Resource.Share.Lib.Responses;
using System.Linq.Expressions;

namespace ProductApi.Infrastructure.Repositories
{
    public class ProductRepository(ProductDbContext context) : IProduct
    {
        public async Task<Response> CreateAsync(Product entity)
        {
            try
            {
                var productExists = await GetAsync(p => p.Name!.Equals(entity.Name));

                if (productExists is not null)
                {
                    return new Response(false, $"{entity.Name} already exists");
                }

                var product = new Product()
                {
                    Name = entity.Name,
                    Price = entity.Price,
                    Quantity = entity.Quantity
                };

                context.Products.Add(product);

                await context.SaveChangesAsync();

                return new Response(true, "Product added successfully");
            }
            catch(Exception ex)
            {
                LogException.LogExceptions(ex);

                return new Response(false, "Error occurred adding new product");
            }

        }

        public async Task<Response> DeleteAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if (product is null)
                {
                    return new Response(false, $"{entity.Name} not found");
                }

                context.Products.Remove(product);
                await context.SaveChangesAsync();

                return new Response(true, $"{entity.Name} deleted successfully");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Error occurred deleting product");
            }
        }

        public async Task<Product> FindByIdAsync(int id)
        {
            try
            {
                var product = await context.Products.FindAsync(id);

                if (product is null)
                {
                    return null!;
                }

                return product;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Error occurred finding product by Id");
            }

        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                return await context.Products.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)    
            {
                LogException.LogExceptions(ex);
                return Enumerable.Empty<Product>();
            }
               
        }

        public async Task<Product> GetAsync(Expression<Func<Product, bool>> predicte)
        {
            try
            {
                var products = await context.Products.Where(predicte).FirstOrDefaultAsync();
                if (products is null)
                {
                    return null!;
                }

                return products;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new InvalidOperationException("Error occurred getting product");
            }

        }

        public async Task<Response> UpdateAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);

                if (product is null)
                {
                    return new Response(false, $"{entity.Name} not found");
                }

                context.Entry(product).State = EntityState.Detached;
                context.Products.Update(entity);
                await context.SaveChangesAsync();

                return new Response(true, $"{entity.Name} updated successfully");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Error occurred updating product");
            }
        }
    }
}
