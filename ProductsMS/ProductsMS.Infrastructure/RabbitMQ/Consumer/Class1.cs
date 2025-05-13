using MongoDB.Driver;
using Newtonsoft.Json;
using ProductsMS.Common.Dtos.Category.Request;
using ProductsMS.Common.Dtos.Category.Response;
using ProductsMS.Common.Dtos.Product.Request;
using ProductsMS.Common.Dtos.Product.Response;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Infrastructure.RabbitMQ.Consumer
{
    public class RabbitMQConsumerCategory
    {
        private readonly RabbitMQConnection _rabbitMQConnection;
        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<CreateCategoryDto> _collection;
        private readonly IMongoCollection<UpdateCategoryDto> _collectionU;
        private readonly IMongoCollection<GetCategoryDto> _collectionG;
        public RabbitMQConsumerCategory(RabbitMQConnection rabbitMQConnection)
        {
            _rabbitMQConnection = rabbitMQConnection;

            // 🔹 Conexión a MongoDB Atlas
            _mongoClient = new MongoClient("mongodb+srv://yadefreitas19:08092001@cluster0.owy2d.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0");
            _database = _mongoClient.GetDatabase("ProductMs");
            _collection = _database.GetCollection<CreateCategoryDto>("Categories");
            _collectionU = _database.GetCollection<UpdateCategoryDto>("Categories");
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
                    var eventMessageC = JsonConvert.DeserializeObject<EventMessage<CreateCategoryDto>>(message);
                    //var eventMessageU = JsonConvert.DeserializeObject<EventMessage<UpdateProductDto>>(message);
                    //var eventMessageD = JsonConvert.DeserializeObject<EventMessage<GetProductDto>>(message);
                    if (eventMessageC?.EventType == "CATEGORY_CREATED")
                    {
                        await _collection.InsertOneAsync(eventMessageC.Data);
                        Console.WriteLine($"Producto insertado en MongoDB: {JsonConvert.SerializeObject(eventMessageC.Data)}");
                    }
                   /* else if (eventMessageU?.EventType == "PRODUCT_UPDATED")
                    {
                        var filter = Builders<UpdateProductDto>.Filter.Eq(p => p.ProductId, eventMessageU.Data.ProductId);
                        var update = Builders<UpdateProductDto>.Update
                            .Set(p => p.ProductName, eventMessageU.Data.ProductName)
                            .Set(p => p.ProductImage, eventMessageU.Data.ProductImage)
                            .Set(p => p.ProductPrice, eventMessageU.Data.ProductPrice)
                            .Set(p => p.ProductDescription, eventMessageU.Data.ProductDescription)
                            .Set(p => p.Avilability, eventMessageU.Data.Avilability)
                            .Set(p => p.ProductStock, eventMessageU.Data.ProductStock)
                            .Set(p => p.CategoryId, eventMessageU.Data.CategoryId);


                        await _collectionU.UpdateOneAsync(filter, update);
                        Console.WriteLine($"Usuario actualizado en MongoDB: {JsonConvert.SerializeObject(eventMessageU.Data)}");
                    }
                    else if (eventMessageD?.EventType == "PRODUCT_DELETED")
                    {
                        var filter = Builders<UpdateProductDto>.Filter.Eq("ProductId", eventMessageD.Data.ProductId);
                        await _collectionU.DeleteOneAsync(filter);
                        Console.WriteLine($"Usuario eliminado en MongoDB con ID: {eventMessageD.Data.UserId}");
                    }*/


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

