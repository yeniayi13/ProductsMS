using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Test.Application
{
    using System;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using ProductosMs.Application;
    using Xunit;

    public class DependencyInjectionTests
    {
        [Fact]
        public void AddApplication_ShouldRegisterMediatR()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddApplication();
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var mediatRService = serviceProvider.GetService<IMediator>();
            Assert.NotNull(mediatRService);
        }
    }
}
