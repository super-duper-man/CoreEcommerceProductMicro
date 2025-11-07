using ProductApi.Domain.Entities;

namespace ProductApi.Application.Dtos.Conversions
{
    public static class ProductConversion
    {
        public static Product ToEntity(ProductDto product) => new()
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Quantity = product.Quantity
        };

        public static (ProductDto?, IEnumerable<ProductDto>?) FromEntity(Product product, IEnumerable<Product>? products)
        {
            if (product is not null || products is null)
            {
                var singleProduct = new ProductDto(
                     product!.Id,
                     product.Name,
                     product.Price,
                     product.Quantity
                    );

                return (singleProduct, null);
            }
            else if (products is not null || product is null)
            {
                var _products = products!.Select(p =>
                                    new ProductDto(p.Id, p.Name, p.Price, p.Quantity)).ToList();
                return (null, _products);
            }
            else
            {
                return (null, null);
            }
        }
    }
}
