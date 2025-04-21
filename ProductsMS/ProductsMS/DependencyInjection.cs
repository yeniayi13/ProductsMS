using ProductosMs.Application.Category.Handlers.Commands;
using ProductsMs.Core.Database;
using ProductsMs.Core.Repository;
using ProductsMs.Infrastructure.Database;
using ProductsMs.Infrastructure.Repositories;
using ProductsMS.Application.Products.Handlers.Queries;

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
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetFilteredProductsQueryHandler).Assembly));
            return services;
        }
    }
}