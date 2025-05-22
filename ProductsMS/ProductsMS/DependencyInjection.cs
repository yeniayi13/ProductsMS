using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ProductsMS;
using ProductsMs.Core.Database;
using ProductsMs.Core.Repository;
using ProductsMs.Infrastructure.Repositories;
using ProductsMS.Application.Products.Handlers.Commands;
using ProductsMS.Application.Products.Handlers.Queries;
using ProductsMS.Infrastructure.Database.Context.Postgres;
using ProductsMS.Infrastructure.Services.User;

namespace ProductosMs
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGenWithAuth(configuration);
            services.KeycloakConfiguration(configuration);

            //* Sin los Scope no funciona!!
            //services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ApplicationDbContext>();
            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
            //Registro de handlers 
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(CreateProductCommandHandler).Assembly));
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(CreateProductCommandHandler).Assembly));
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(UpdateProductCommandHandler).Assembly));
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetNameProductQueryHandler).Assembly));
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetAvailableProductsQueryHandler).Assembly));
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetProductQueryHandler).Assembly));
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetAllProductQueryHandler).Assembly));
            //services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetFilteredProductsQueryHandler).Assembly));
            services.AddHttpClient<UserService>(
                client =>
                {
                    client.BaseAddress = new Uri("https://localhost:18084");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                }
            );
            return services;
        }
    }
}