using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.Dtos;
using ProductApi.Application.Dtos.Conversions;
using ProductApi.Application.Interfaces;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProduct productInterface) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<ProductDto>> GetProducts()
        {
            var products = await productInterface.GetAllAsync();

            if (!products.Any())
            {
                return NotFound("No Product Found!");
            }

            var (_, list) = ProductConversion.FromEntity(null, products);

            return list!.Any() ? Ok(list) : NotFound();
        }
    }
}
