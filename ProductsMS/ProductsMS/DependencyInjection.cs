using Microsoft.Extensions.DependencyInjection;
using ProductosMs.Application.Category.Handlers.Commands;
using ProductsMs.Core.Database;
using ProductsMs.Core.Repository;
using ProductsMs.Infrastructure.Repositories;
using ProductsMS.Application.Products.Handlers.Commands;
using ProductsMS.Application.Products.Handlers.Queries;
using ProductsMS.Infrastructure.Database.Context.Postgres;

namespace ProductosMs
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            //services.AddSwaggerGenWithAuth(configuration);
            //services.KeycloakConfiguration(configuration);

            //* Sin los Scope no funciona!!
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            //Registro de handlers 
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(CreateCategoryCommandHandler).Assembly));
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(CreateCategoryCommandHandler).Assembly));
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(UpdateCategoryCommandHandler).Assembly));
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(CreateProductCommandHandler).Assembly));
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(CreateProductCommandHandler).Assembly));
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(UpdateProductCommandHandler).Assembly));
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetNameProductQueryHandler).Assembly));
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetAvailableProductsQueryHandler).Assembly));
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetProductQueryHandler).Assembly));
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetAllProductQueryHandler).Assembly));
            //services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetFilteredProductsQueryHandler).Assembly));
            return services;
        }
    }
}