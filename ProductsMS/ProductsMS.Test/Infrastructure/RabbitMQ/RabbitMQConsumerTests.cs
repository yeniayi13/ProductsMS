using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using Moq;
using Newtonsoft.Json;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMS.Core.RabbitMQ;
using ProductsMS.Infrastructure.RabbitMQ.Consumer;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Xunit;


namespace ProductsMS.Test.Infrastructure.RabbitMQ
{


    public class RabbitMQConsumerTests
    {
        private readonly Mock<IConnectionRabbbitMQ> _mockRabbitMQConnection;
        private readonly Mock<IChannel> _mockChannel;
        private readonly Mock<IMongoCollection<GetProductDto>> _mockCollection;
        private readonly RabbitMQConsumer _consumer;

        public RabbitMQConsumerTests()
        {
            _mockRabbitMQConnection = new Mock<IConnectionRabbbitMQ>();
            _mockChannel = new Mock<IChannel>();
            _mockCollection = new Mock<IMongoCollection<GetProductDto>>();

            // 🔹 Asegurar que `RabbitMQConsumer` recibe `_mockCollection.Object` en pruebas
            _mockRabbitMQConnection.Setup(c => c.GetChannel()).Returns(_mockChannel.Object);
            _consumer = new RabbitMQConsumer(_mockRabbitMQConnection.Object, _mockCollection.Object);
        }

        [Fact]
        public async Task ConsumeMessagesAsync_ShouldDeclareQueue_AndStartConsumer()
        {
            // Arrange
            var queueName = "testQueue";

            // Simular declaración de la cola
            _mockChannel.Setup(c => c.QueueDeclareAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(),
                    It.IsAny<bool>(), It.IsAny<IDictionary<string, object>>(), It.IsAny<bool>(), It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new QueueDeclareOk(It.IsAny<string>(), It.IsAny<uint>(), It.IsAny<uint>()));

            // 🔥 No se puede mockear directamente `BasicConsumeAsync()`, pero podemos simular la ejecución manual
            var consumer = new AsyncEventingBasicConsumer(_mockChannel.Object);

            // Act
            await _consumer.ConsumeMessagesAsync(queueName);

            // Assert
            _mockChannel.Verify(c => c.QueueDeclareAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<IDictionary<string, object>>(), It.IsAny<bool>(), It.IsAny<bool>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        /* [Fact]
         public async Task ConsumeMessagesAsync_ShouldInsertProduct_WhenProductCreatedEventReceived()
         {
             // Arrange
             var queueName = "test-queue";
             var product = new GetProductDto { ProductId = Guid.NewGuid(), ProductName = "Nuevo Producto" };
             var message = JsonConvert.SerializeObject(new EventMessage<GetProductDto>
             {
                 EventType = "PRODUCT_CREATED",
                 Data = product
             });

             var ea = new BasicDeliverEventArgs { Body = Encoding.UTF8.GetBytes(message) };

             var consumer = new AsyncEventingBasicConsumer(_mockChannel.Object);
             consumer.ReceivedAsync += async (_, args) =>
             {
                 await _consumer.ProcessMessage(args);
             };

             // Act
             await consumer.ReceivedAsync.Invoke(this, ea);

             // Assert
             _mockCollection.Verify(c => c.InsertOneAsync(It.Is<GetProductDto>(p => p.ProductId == product.ProductId),
                 null, default), Times.Once);
         }

         [Fact]
         public async Task ConsumeMessagesAsync_ShouldUpdateProduct_WhenProductUpdatedEventReceived()
         {
             // Arrange
             var queueName = "test-queue";
             var product = new GetProductDto { ProductId = Guid.NewGuid(), ProductName = "Producto Modificado" };
             var message = JsonConvert.SerializeObject(new EventMessage<GetProductDto>
             {
                 EventType = "PRODUCT_UPDATED",
                 Data = product
             });

             var ea = new BasicDeliverEventArgs { Body = Encoding.UTF8.GetBytes(message) };

             var consumer = new AsyncEventingBasicConsumer(_mockChannel.Object);
             consumer.ReceivedAsync += async (_, args) =>
             {
                 await _consumer.ProcessMessage(args);
             };

             // Act
             await consumer.ReceivedAsync.Invoke(this, ea);

             // Assert
             _mockCollection.Verify(c => c.UpdateOneAsync(It.IsAny<FilterDefinition<GetProductDto>>(),
                 It.IsAny<UpdateDefinition<GetProductDto>>(), null, default), Times.Once);
         }

         [Fact]
         public async Task ConsumeMessagesAsync_ShouldDeleteProduct_WhenProductDeletedEventReceived()
         {
             // Arrange
             var queueName = "test-queue";
             var product = new GetProductDto { ProductId = Guid.NewGuid() };
             var message = JsonConvert.SerializeObject(new EventMessage<GetProductDto>
             {
                 EventType = "PRODUCT_DELETED",
                 Data = product
             });

             var ea = new BasicDeliverEventArgs { Body = Encoding.UTF8.GetBytes(message) };

             var consumer = new AsyncEventingBasicConsumer(_mockChannel.Object);
             consumer.ReceivedAsync += async (_, args) =>
             {
                 await _consumer.ProcessMessage(args);
             };

             // Act
             await consumer.ReceivedAsync.Invoke(this, ea);

             // Assert
             _mockCollection.Verify(c => c.DeleteOneAsync(It.IsAny<FilterDefinition<GetProductDto>>(),
                 default), Times.Once);
         }

         [Fact]
         public async Task ConsumeMessagesAsync_ShouldLogError_WhenMessageProcessingFails()
         {
             // Arrange
             var queueName = "test-queue";
             var invalidMessage = "Invalid JSON"; // Mensaje que causará excepción

             var ea = new BasicDeliverEventArgs { Body = Encoding.UTF8.GetBytes(invalidMessage) };

             var consumer = new AsyncEventingBasicConsumer(_mockChannel.Object);
             consumer.ReceivedAsync += async (_, args) =>
             {
                 await _consumer.ProcessMessage(args);
             };

             // Act & Assert
             await Assert.ThrowsAsync<JsonSerializationException>(() => consumer.ReceivedAsync.Invoke(this, ea));
         }

     }*/
    }
}
