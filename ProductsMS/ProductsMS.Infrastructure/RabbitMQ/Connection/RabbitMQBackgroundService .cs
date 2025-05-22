
using Microsoft.Extensions.Hosting;
using ProductsMS.Core.RabbitMQ;
using ProductsMS.Infrastructure.RabbitMQ.Consumer;

namespace ProductsMS.Infrastructure.RabbitMQ.Connection
{
    public class RabbitMQBackgroundService : BackgroundService
    {
        private readonly IRabbitMQConsumer _rabbitMQConsumer;

        public RabbitMQBackgroundService(IRabbitMQConsumer rabbitMQConsumer)
        {
            _rabbitMQConsumer = rabbitMQConsumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine(" Esperando la inicialización de RabbitMQ...");

            await Task.Delay(3000); // Pequeño retraso para asegurar la inicialización
            await _rabbitMQConsumer.ConsumeMessagesAsync("productQueue");

            Console.WriteLine(" Consumidor de RabbitMQ iniciado.");
        }
    }
}
