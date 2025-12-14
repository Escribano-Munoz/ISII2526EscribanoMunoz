using System;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LogViewer
{
    public class Subscriber
    {
        private readonly string _hostname = "192.168.1.130";
        private readonly string _exchangeName = "logs-topic";
        private readonly string _topicPattern;
        private readonly string _userName = "guest";
        private readonly string _password = "guest";
        private readonly int _port = 5672;

        private IConnection _connection;
        private IModel _channel;

        public Subscriber(string topicPattern = "#")
        {
            _topicPattern = topicPattern;
        }

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

            _channel.ExchangeDeclare(
                exchange: _exchangeName,
                type: ExchangeType.Topic,
                durable: true
            );

            var tempQueue = _channel.QueueDeclare();
            var queueName = tempQueue.QueueName;

            _channel.QueueBind(
                queue: queueName,
                exchange: _exchangeName,
                routingKey: _topicPattern
            );

            Console.WriteLine($"Suscrito a: {_exchangeName}");
            Console.WriteLine($"Topic: {_topicPattern}");
            Console.WriteLine($"Cola: {queueName}");
            Console.WriteLine($"Esperando logs...\n");

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);

                try
                {
                    using var doc = JsonDocument.Parse(messageJson);
                    var root = doc.RootElement;

                    Console.WriteLine($"─── LOG [{ea.RoutingKey}] ───");

                    if (root.TryGetProperty("Timestamp", out var timestamp))
                        Console.WriteLine($"Hora: {timestamp}");

                    if (root.TryGetProperty("Level", out var level))
                        Console.WriteLine($"Nivel: {level}");

                    if (root.TryGetProperty("Message", out var message))
                        Console.WriteLine($"Mensaje: {message}");

                    Console.WriteLine($"─────────────────────────────\n");
                }
                catch
                {
                    Console.WriteLine($"[RAW] {messageJson}\n");
                }
            };

            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            Console.WriteLine("Presiona ENTER para salir.");
            Console.ReadLine();

            _channel?.Close();
            _connection?.Close();
        }
    }
}
