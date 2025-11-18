using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.Dtos;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Presentation.Controllers;
using Resource.Share.Lib.Responses;

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

        [Fact]
        public async Task GetProduct_WhenNoProductExits_ShouldReturnNotFoundResult()
        {
            //Arange
            var products = new List<Product>();

            //Set up fake response
            A.CallTo(() => productInterface.GetAllAsync()).Returns(products);

            //Act
            var result  = await productController.GetProducts();

            //Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);

            var message = notFoundResult.Value as string;
            message.Should().Be("No Product Found!");
        }

        [Fact]
        public async Task CreateProduct_WhenModelStateIsInvalid_ReturnBadRequest()
        {
            //Arrange
            var productDto = new ProductDto(1,"Product 1", 34.95m, 67);

            //Set up invalid model state
            productController.ModelState.AddModelError("Name", "The Name field is required.");

            //Act
            var result = await productController.CreateProduct(productDto);

            //Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task CreateProduct_WhenCreateIsSuccessfull_ReturnReponse()
        {
            //Arrange
            var productDto = new ProductDto(1, "Product 1", 34.95m, 67);
            var response = new Response(true, "Product created successfully.");

            //Act
            A.CallTo(() => productInterface.CreateAsync(A<Product>.Ignored)).Returns(response);
            var result = await productController.CreateProduct(productDto);

            //Assert
            var okResutl = result.Result as OkObjectResult;
            okResutl.Should().NotBeNull();
            okResutl.StatusCode.Should().Be(StatusCodes.Status200OK);

            var returnedResponse = okResutl.Value as Response;
            returnedResponse!.Message.Should().Be("Product created successfully.");
            returnedResponse!.Flag.Should().Be(true);
        }

        [Fact]
        public async Task CreateProduct_WhenCreateFails_ReturnBadRequestResponse()
        {
            //Arrange
            var productDto = new ProductDto(1, "Product 1", 34.95m, 67);
            var response = new Response(false, "Error occured during creating product");

            //Act
            A.CallTo(() => productInterface.CreateAsync(A<Product>.Ignored)).Returns(response);
            var result = await productController.CreateProduct(productDto);

            //Assert
            var returnedResponse = result.Result as BadRequestObjectResult;
            returnedResponse.Should().NotBeNull();
            returnedResponse.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var badRequestResponse = returnedResponse.Value as Response;
            badRequestResponse!.Message.Should().Be("Error occured during creating product");
            badRequestResponse!.Flag.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateProduct_WhenUpdateIsSuccessfull_ReturnOkReponse()
        {
            //Arrange
            var productDto = new ProductDto(1, "Product 1", 34.95m, 67);
            var response = new Response(true, "Update product successfully");

            //Act
            A.CallTo(() => productInterface.UpdateAsync(A<Product>.Ignored)).Returns(response);
            var result = await productController.UpdateProduct(productDto);

            //Assert
            var returnResul = result.Result as OkObjectResult;
            returnResul.Should().NotBeNull();
            returnResul.StatusCode.Should().Be(StatusCodes.Status200OK);

            var okResponse = returnResul.Value as Response;
            okResponse!.Message.Should().Be("Update product successfully");
            okResponse!.Flag.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateProduct_WhenUpdateFails_ReturnBadRequestReponse()
        {
            //Arrange
            var productDto = new ProductDto(1, "Product 1", 34.95m, 67);
            var response = new Response(false, "Bad Request");

            //Act
            A.CallTo(() => productInterface.UpdateAsync(A<Product>.Ignored)).Returns(response);
            var result = await productController.UpdateProduct(productDto);

            //Assert
            var returnResul = result.Result as BadRequestObjectResult;
            returnResul.Should().NotBeNull();
            returnResul.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var okResponse = returnResul.Value as Response;
            okResponse!.Message.Should().Be("Bad Request");
            okResponse!.Flag.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteProduct_WhenDeleteIsSuccessfull_ReturnOkResponse()
        {
            //Arrange
            var productDto = new ProductDto(1, "Product 1", 34.95m, 67);
            var response = new Response(true, "Delete Successfully");

            //Act
            A.CallTo(() => productInterface.DeleteAsync(A<Product>.Ignored)).Returns(response);
            var result = await productController.DeleteProduct(productDto);

            //Assert
            var returnResul = result.Result as OkObjectResult;
            returnResul.Should().NotBeNull();
            returnResul.StatusCode.Should().Be(StatusCodes.Status200OK);

            var okResult = returnResul.Value as Response;
            okResult!.Message.Should().Be("Delete Successfully");
            okResult!.Flag.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteProduct_WhenDeleteFails_ReturnBadRequestResponse()
        {
            //Arrange
            var productDto = new ProductDto(1, "Product 1", 34.95m, 67);
            var response = new Response(false, "Delete Failed!");

            //Act
            A.CallTo(() => productInterface.DeleteAsync(A<Product>.Ignored)).Returns(response);
            var result = await productController.DeleteProduct(productDto);

            //Assert
            var returnResul = result.Result as BadRequestObjectResult;
            returnResul.Should().NotBeNull();
            returnResul.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var okResult = returnResul.Value as Response;
            okResult!.Message.Should().Be("Delete Failed!");
            okResult!.Flag.Should().BeFalse();
        }
    }
}
