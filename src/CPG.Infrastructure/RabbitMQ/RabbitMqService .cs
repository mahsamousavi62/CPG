using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Infrastructure.RabbitMQ;

public class RabbitMqService : IDisposable, IRabbitMqService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqService(IConfiguration configuration)
    {
        var rabbitMqConfig = configuration.GetSection("Infrastructure:RabbitMQ").Get<RabbitMqConfig>();

        var factory = new ConnectionFactory
        {
            HostName = rabbitMqConfig.HostName,
            Port = rabbitMqConfig.Port,
            UserName = rabbitMqConfig.UserName,
            Password = rabbitMqConfig.Password
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public RabbitMqService(string hostName, int port, string userName, string password)
    {
        var factory = new ConnectionFactory
        {
            HostName = hostName,
            Port = port,
            UserName = userName,
            Password = password
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void PublishMessage(string exchange, string routingKey, string message)
    {

        _channel.QueueDeclare(queue: routingKey,
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: string.Empty,
                             routingKey: routingKey,
                             basicProperties: null,
                             body: body);
    }

    public async Task<string> ConsumeMessage(string queue)
    {
        string message = string.Empty;

        _channel.QueueDeclare(queue: queue,
                 durable: false,
                 exclusive: false,
                 autoDelete: false,
                 arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            message = Encoding.UTF8.GetString(body);
        };

        _channel.BasicConsume(queue: queue,
                              autoAck: true,
                              consumer: consumer);
        await Task.CompletedTask;
        return message;
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }
}