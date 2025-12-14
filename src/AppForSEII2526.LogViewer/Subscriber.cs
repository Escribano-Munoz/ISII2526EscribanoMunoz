using System;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LogViewer
{
    public class Subscriber
    {
        private readonly string _hostname = "localhost";
        private readonly string _exchangeName = "logs";
        private readonly string _userName = "guest";
        private readonly string _password = "guest";
        private readonly int _port = 5672;

        private IConnection _connection;
        private IModel _channel;

        public void Start()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _hostname,
                UserName = _userName,
                Password = _password,
                Port = _port
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Fanout, durable: true);

            var tempQueue = _channel.QueueDeclare();
            var queueName = tempQueue.QueueName;

            _channel.QueueBind(queue: queueName, exchange: _exchangeName, routingKey: "");

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);

                try
                {
                    using JsonDocument doc = JsonDocument.Parse(messageJson);
                    var root = doc.RootElement;

                    Console.WriteLine($"----LOG----");

                    if (root.TryGetProperty("Timestamp", out var timestamp))
                        Console.WriteLine($"Hora: {timestamp}");

                    if (root.TryGetProperty("Level", out var level))
                        Console.WriteLine($"Nivel: {level}");

                    if (root.TryGetProperty("Message", out var message))
                        Console.WriteLine($"Mensaje: {message}");

                    if (root.TryGetProperty("Category", out var category))
                        Console.WriteLine($"Categoría: {category}");

                    Console.WriteLine($"-----------");
                }
                catch (JsonException)
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {messageJson}");
                }
            };

            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            Console.WriteLine("Esperando logs... Presiona ENTER para salir.");
            Console.ReadLine();

            _channel?.Close();
            _connection?.Close();
        }
    }
}
