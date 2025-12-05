using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Consumer;

namespace ProductApi.Application.DependencyInjection
{
    public static class ApplicationServiceContainer
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddMassTransit(x => {
                x.AddConsumer<ProductIdConsumer>();
                x.UsingRabbitMq((context, cfg) => {
                    cfg.Host("rabbitmq://localhost", h => {
                        h.Username("guest");
                        h.Password("guest");
                    });
                    cfg.ReceiveEndpoint("product-id", e => {
                        e.ConfigureConsumer<ProductIdConsumer>(context);
                    });
                });
            });

            return services;
        }
    }
}
