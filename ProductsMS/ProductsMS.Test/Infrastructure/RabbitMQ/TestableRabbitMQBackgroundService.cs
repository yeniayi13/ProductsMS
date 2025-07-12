using ProductsMS.Infrastructure.RabbitMQ.Connection;
using ProductsMS.Infrastructure.RabbitMQ.Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductsMS.Core.RabbitMQ;

namespace ProductsMS.Test.Infrastructure.RabbitMQ
{
    public class TestableRabbitMQBackgroundService : RabbitMQBackgroundService
    {
        public TestableRabbitMQBackgroundService(IRabbitMQConsumer rabbitMQConsumer)
            : base(rabbitMQConsumer) { }

        public async Task TestExecuteAsync(CancellationToken token)
        {
            await ExecuteAsync(token); // Exponer el método protegido para pruebas
        }
    }
}
