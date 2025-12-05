using MassTransit;
using ProductApi.Application.Interfaces;
using Resource.Share.Lib.MqRequests;

namespace ProductApi.Application.Consumer
{
    public class ProductIdConsumer(IProduct productInterface, IPublishEndpoint publisher) : IConsumer<ProductRequest>
    {
        public async Task Consume(ConsumeContext<ProductRequest> context)
        {
            var product = await productInterface.FindByIdAsync(context.Message.Id);
            await publisher.Publish(new ProductResponse(product.Id, product.Name, product.Quantity, product.Price));
        }
    }
}
