using Microsoft.EntityFrameworkCore.Metadata;
using ProductsMS.Core.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class RabbitMQConnection : IConnectionRabbbitMQ
{
    private IConnection _connection;
    private IChannel _channel;
    private readonly IConnectionFactory _connectionFactory;

    public RabbitMQConnection(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task InitializeAsync()
    {
        int retries = 10;
        int delay = 3000; // 3 seconds

        for (int i = 0; i < retries; i++)
        {
            try
            {
                _connection = await _connectionFactory.CreateConnectionAsync(CancellationToken.None);

                if (_connection == null)
                {
                    throw new InvalidOperationException("No se pudo establecer la conexión con RabbitMQ.");
                }

                _channel = await _connection.CreateChannelAsync();

                if (_channel == null)
                {
                    throw new InvalidOperationException("No se pudo crear el canal de comunicación con RabbitMQ.");
                }

                await _channel.QueueDeclareAsync("productQueue", true, false, false);
                return; // Success!
            }
            catch (Exception ex)
            {
                if (i == retries - 1)
                    throw; // Last try, rethrow
                Console.WriteLine($"RabbitMQ not ready, retrying in {delay / 1000} seconds... ({ex.Message})");
                await Task.Delay(delay);
            }
        }
    }

    public IChannel GetChannel()
    {
        if (_channel == null)
        {
            throw new InvalidOperationException("RabbitMQ aún no está inicializado correctamente.");
        }
        return _channel;
    }

   

}