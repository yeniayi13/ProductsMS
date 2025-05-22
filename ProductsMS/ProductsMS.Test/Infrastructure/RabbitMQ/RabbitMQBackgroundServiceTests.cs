using Xunit;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using ProductsMS.Infrastructure.RabbitMQ.Connection;
using ProductsMS.Infrastructure.RabbitMQ.Consumer;

namespace ProductsMS.Test.Infrastructure.RabbitMQ
{
    using Xunit;
    using Moq;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using ProductsMS.Core.RabbitMQ;

    public class RabbitMQBackgroundServiceTests
    {
        private readonly Mock<IRabbitMQConsumer> _mockConsumer;
        private readonly RabbitMQBackgroundService _service;

        public RabbitMQBackgroundServiceTests()
        {
            _mockConsumer = new Mock<IRabbitMQConsumer>();
            _mockConsumer.Setup(c => c.ConsumeMessagesAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
            _service = new RabbitMQBackgroundService(_mockConsumer.Object);
        }

        [Fact]
        public async Task StartAsync_ShouldCall_ConsumeMessagesAsync()
        {
            // Arrange
            var cancellationTokenSource = new CancellationTokenSource();

            // Act
            await _service.StartAsync(cancellationTokenSource.Token);

            // Esperar un poco más para asegurar que `ExecuteAsync` tuvo tiempo de ejecutarse
            await Task.Delay(3500);

            // Assert
            _mockConsumer.Verify(c => c.ConsumeMessagesAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldNotFail_WhenCancellationRequested()
        {
            // Arrange
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel(); // Simular que el token de cancelación ha sido activado

            // Act & Assert
            await _service.StartAsync(cancellationTokenSource.Token);

            // Verificar que el servicio no intentó llamar a `ConsumeMessagesAsync`
            _mockConsumer.Verify(c => c.ConsumeMessagesAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldCall_ConsumeMessagesAsync()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var service = new TestableRabbitMQBackgroundService(_mockConsumer.Object);

            // Act
            await service.TestExecuteAsync(cancellationToken);

            // Assert
            _mockConsumer.Verify(c => c.ConsumeMessagesAsync(It.IsAny<string>()), Times.Once);
        }
        [Fact]
        public async Task ExecuteAsync_ShouldWaitBeforeConsumingMessages()
        {
            // Arrange
            var cancellationToken = CancellationToken.None; // No cancelar ejecución
            var service = new TestableRabbitMQBackgroundService(_mockConsumer.Object);

            // Act - Ejecutar la tarea en paralelo para observar el comportamiento
            var executionTask = service.TestExecuteAsync(cancellationToken);

            //  Esperar **un poco más** antes de la primera verificación para evitar errores
            await Task.Delay(1000); // Ajusta este tiempo según el comportamiento real de la ejecución

            // Verificar que `ConsumeMessagesAsync` aún no ha sido llamado inmediatamente
            _mockConsumer.Verify(c => c.ConsumeMessagesAsync(It.IsAny<string>()), Times.Never);

            //  Esperar la ejecución completa antes de la verificación final
            await executionTask;

            // Verificar que `ConsumeMessagesAsync` se haya ejecutado después del retraso
            _mockConsumer.Verify(c => c.ConsumeMessagesAsync(It.IsAny<string>()), Times.Once);
        }
    }
}
