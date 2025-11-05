using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Interfaces;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;
using Resource.Share.Lib.DependencyInjection;

namespace ProductApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
        {
             ShareServiceContainer.AddShareService<ProductDbContext>(services, config, config["AppSerilog:FileName"]!);

            services.AddScoped<IProduct, ProductRepository>();

            return services;
        }

        public static IApplicationBuilder UseInfrastracturePolicies(this IApplicationBuilder app)
        {
            ShareServiceContainer.UseSharedPolicies(app);

            return app;
        }
    }
}
