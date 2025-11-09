using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.Dtos;
using ProductApi.Application.Dtos.Conversions;
using ProductApi.Application.Interfaces;
using Resource.Share.Lib.Responses;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProduct productInterface) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await productInterface.GetAllAsync();

            if (!products.Any())
            {
                return NotFound("No Product Found!");
            }

            var (_, list) = ProductConversion.FromEntity(null!, products);

            return list!.Any() ? Ok(list) : NotFound();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await productInterface.FindByIdAsync(id);

            if (product is null)
            {
                return NotFound("Product Not Found!");
            }

            var (_product, _) = ProductConversion.FromEntity(product, null);

            return _product is not null ? Ok(_product) : NotFound("Product Not Found!");
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateProduct([FromBody] ProductDto product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.CreateAsync(productEntity);

            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateProduct([FromBody] ProductDto product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.UpdateAsync(productEntity);

            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        public async Task<ActionResult<Response>> DeleteProduct([FromBody] ProductDto product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.DeleteAsync(productEntity);

            return response.Flag is true ? Ok(response) : BadRequest(response);
        }
    }
}
