using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.Dtos;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Presentation.Controllers;

namespace UnitTest.ProductApi.Controllers
{
    public class ProdcutControllerTest
    {
        private readonly IProduct productInterface;
        private readonly ProductController productController;

        public ProdcutControllerTest()
        {
            productInterface = A.Fake<IProduct>();
            productController = new ProductController(productInterface);
        }

        //Get All Products
        [Fact]
        public async Task GetProduct_WhenProductExits_ShouldReturnOkResultWithProducts()
        {
            //Arrange
            var products = new List<Product>()
            {
                new Product { Id = 1, Name = "Product1", Price = 10.0m, Quantity = 10 },
                new Product { Id = 2, Name = "Product2", Price = 20.0m, Quantity = 110 }
            };

            //set up fake response
            A.CallTo(() => productInterface.GetAllAsync()).Returns(products);

            //Act
            var result = await productController.GetProducts();

            //Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            var returnedProducts = okResult.Value as IEnumerable<ProductDto>;
            returnedProducts.Should().NotBeNull();
            returnedProducts.Should().HaveCount(2);
            returnedProducts.First().Id.Should().Be(1);
            returnedProducts.Last().Id.Should().Be(2);

        }
    }
}
