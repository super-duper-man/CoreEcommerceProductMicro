using FakeItEasy;
using ProductApi.Application.Interfaces;
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
    }
}
