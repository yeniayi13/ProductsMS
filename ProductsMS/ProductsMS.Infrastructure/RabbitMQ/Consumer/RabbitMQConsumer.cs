//using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using RabbitMQ.Client;
using Newtonsoft.Json;
using MongoDB.Driver;
using ProductsMS.Common.Dtos.Product.Request;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMs.Domain.Entities.Products;
using ProductsMS.Common.Dtos.Category.Request;

namespace ProductsMS.Infrastructure.RabbitMQ.Consumer
{
    public class RabbitMQConsumer
    {
        private readonly RabbitMQConnection _rabbitMQConnection;
        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<GetProductDto> _collection;
        public RabbitMQConsumer(RabbitMQConnection rabbitMQConnection)
        {
            _rabbitMQConnection = rabbitMQConnection;

            // 🔹 Conexión a MongoDB Atlas
            _mongoClient = new MongoClient("mongodb+srv://yadefreitas19:08092001@cluster0.owy2d.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0");
            _database = _mongoClient.GetDatabase("ProductMs");
            _collection = _database.GetCollection<GetProductDto>("Products");

        }
        

        public async Task ConsumeMessagesAsync(string queueName)
        {
            var channel = _rabbitMQConnection.GetChannel();
            await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Mensaje recibido: {message}");

                try
                {
                    var eventMessageD = JsonConvert.DeserializeObject<EventMessage<GetProductDto>>(message);
                    if (eventMessageD?.EventType == "PRODUCT_CREATED")
                    {
                        await _collection.InsertOneAsync(eventMessageD.Data);
                        Console.WriteLine($"Producto insertado en MongoDB: {JsonConvert.SerializeObject(eventMessageD.Data)}");
                    }
                    else if (eventMessageD?.EventType == "PRODUCT_UPDATED")
                    {
                        var filter = Builders<GetProductDto>.Filter.Eq(p => p.ProductId, eventMessageD.Data.ProductId);
                        var update = Builders<GetProductDto>.Update
                            .Set(p => p.ProductName, eventMessageD.Data.ProductName)
                            .Set(p => p.ProductImage, eventMessageD.Data.ProductImage)
                            .Set(p => p.ProductPrice, eventMessageD.Data.ProductPrice)
                            .Set(p => p.ProductDescription, eventMessageD.Data.ProductDescription)
                            .Set(p => p.ProductAvilability, eventMessageD.Data.ProductAvilability)
                            .Set(p => p.ProductStock, eventMessageD.Data.ProductStock)
                            .Set(p => p.CategoryId, eventMessageD.Data.CategoryId)
                            .Set(p => p.ProductUserId, eventMessageD.Data.ProductUserId);



                        await _collection.UpdateOneAsync(filter, update);
                        Console.WriteLine($"Usuario actualizado en MongoDB: {JsonConvert.SerializeObject(eventMessageD.Data)}");
                    }
                    else if (eventMessageD?.EventType == "PRODUCT_DELETED")
                    {
                        var filter = Builders<GetProductDto>.Filter.Eq("ProductId", eventMessageD.Data.ProductId);
                        await _collection.DeleteOneAsync(filter);
                        Console.WriteLine($"Usuario eliminado en MongoDB con ID: {eventMessageD.Data.ProductUserId}");
                    }
                    else if (eventMessageD?.EventType == "CATEGORY_CREATED")
                    {
                        await _collection.InsertOneAsync(eventMessageD.Data);
                        Console.WriteLine($"Category insertado en MongoDB: {JsonConvert.SerializeObject(eventMessageD.Data)}");
                    }


                    await Task.Run(() => channel.BasicAckAsync(ea.DeliveryTag, false));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error procesando el mensaje: {ex.Message}");
                }
            };

            await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
            Console.WriteLine("Consumidor de RabbitMQ escuchando mensajes...");
        }
    }
}