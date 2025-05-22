using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProductsMS.Application;

namespace ProductosMs.Application
{
    
    public static class DependecyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(config =>{
                config.RegisterServicesFromAssemblyContaining<ApplicationAssemblyRef>();
            });

            //services.AddValidatorsFromAssemblyContaining<ApplicationAssemblyRef>();
            return services;
        }
    }
}