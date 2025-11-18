using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;
using System.Linq.Expressions;

namespace UnitTest.ProductApi.Repositories
{
    public class ProductRepositoryTest
    {
        public readonly ProductDbContext productDbContext;
        private readonly ProductRepository productRepository;

        public ProductRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(databaseName: "ProductDb_Test").Options;

            productDbContext = new ProductDbContext(options);
            productRepository = new ProductRepository(productDbContext);
        }

        [Fact]
        public async Task CreateProduct_WhenProductAlreadyExists_ShouldReturnErrorReponse()
        {
            //Arrange
            var existingProduct = new Product() { Name = "ExistingProduct" };
            productDbContext.Products.Add(existingProduct);
            await productDbContext.SaveChangesAsync();

            //Act
            var result = await productRepository.CreateAsync(existingProduct);

            //Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
            result.Message.Should().Be("ExistingProduct already exists");
        }

        [Fact]
        public async Task CreateProduct_WhenProductDoesNotWExists_ShouldAddProductAndReturnOkReposnse()
        {
            //Arrange
            var newProduct = new Product() { Name = "New Product" };
            //Act
            var result = await productRepository.CreateAsync(newProduct);

            //Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
            result.Message.Should().Be("Product added successfully");
        }

        [Fact]
        public async Task DeleteProduct_WhenProductIsFound_ShouldReturnSuccessReponse()
        {
            //Arrange
            var newProduct = new Product() { Id = 1, Name = "New Product", Price = 75.65m, Quantity = 2400 };
            productDbContext.Products.Add(newProduct);

            //Act
            var result = await productRepository.DeleteAsync(newProduct);

            //Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
            result.Message.Should().Be("New Product deleted successfully");
        }

        [Fact]
        public async Task DeleteProduct_WhenProductNotFound_ShouldReturnNotReponse()
        {
            //Arrange
            var exitsProduct = new Product() { Id = 30, Name = "NotFoud Product", Price = 750.65m, Quantity = 240 };

            //Act
            var result = await productRepository.DeleteAsync(exitsProduct);

            //Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
            result.Message.Should().Be("NotFoud Product not found");
        }

        [Fact]
        public async Task GetProduct_ById_WhenProductIsFound_ReturnProduct()
        {
            //Arrange
            var newProduct = new Product() { Id = 1, Name = "New Product", Price = 75.65m, Quantity = 2400 };
            productDbContext.Products.Add(newProduct);
            await productDbContext.SaveChangesAsync();

            //Act
            var result = await productRepository.FindByIdAsync(1);

            //Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("New Product");
        }

        [Fact]
        public async Task GetProduct_ById_WhenProductIsNotFound_ReturnNull()
        {
            //Arrange

            //Act
            var result = await productRepository.FindByIdAsync(1);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllProduct_ById_WhenProductsFound_ReturnProducts()
        {
            //Arrange
            var newProducts = new List<Product> {
                 new() { Name = "Product 1" },
                 new() { Name = "Product 2" },
                 new() { Name = "Product 3" },
                 new() { Name = "Product 4" },
            };
            productDbContext.Products.AddRange(newProducts);
            await productDbContext.SaveChangesAsync();

            //Act
            var result = await productRepository.GetAllAsync();

            //Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(4);
            result.Should().Contain(p => p.Name == "Product 1");
            result.Should().Contain(p => p.Name == "Product 2");
            result.Should().Contain(p => p.Name == "Product 3");
            result.Should().Contain(p => p.Name == "Product 4");
        }

        [Fact]
        public async Task GetAllProduct_ById_WhenProductsAreNotFound_ReturnNull()
        {
            //Arrange

            //Act
            var result = await productRepository.GetAllAsync();

            //Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetProduct_ByAnyType_WhenProductsIsFound_ReturnProduct()
        {
            //Arrange
            var newProduct = new Product() { Id = 1, Name = "New Product", Price = 75.65m, Quantity = 2400 };
            productDbContext.Products.Add(newProduct);
            await productDbContext.SaveChangesAsync();
            Expression<Func<Product, bool>> predicate = p => p.Name == "New Product";

            //Act
            var result = await productRepository.GetAsync(predicate);

            //Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("New Product");
        }

        [Fact]
        public async Task GetProduct_ByAnyType_WhenProductsIsNotFound_ReturnNull()
        {
            //Arrange
            Expression<Func<Product, bool>> predicate = p => p.Name == "New Product";

            //Act
            var result = await productRepository.GetAsync(predicate);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateProduct_WhenProductFound_ReturnSuccessReponse()
        {
            //Arrange
            var product = new Product() { Id = 1, Name = "New Product", Price = 75.65m, Quantity = 2400 };
            productDbContext.Products.Add(product);
            await productDbContext.SaveChangesAsync();

            //Act
            var result = await productRepository.UpdateAsync(new Product() { Id = 1, Name = "Product 1", Price = 71.605m, Quantity = 2400 });

            //Assert
            result.Should().NotBeNull();
            result.Flag.Should().Be(true);
            result.Message.Should().Be("Product 1 updated successfully");
        }

        [Fact]
        public async Task UpdateProduct_WhenProductIsNotFound_ReturnErroReponse()
        {
            //Arrange
            var product = new Product() { Id = 1, Name = "New Product", Price = 75.65m, Quantity = 2400 };

            //Act
            var result = await productRepository.UpdateAsync(product);

            //Assert
            result.Should().NotBeNull();
            result.Flag.Should().Be(false);
            result.Message.Should().Be("New Product not found");
        }
    }
}
