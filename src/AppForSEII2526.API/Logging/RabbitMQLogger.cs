using AppForSEII2526.Logging;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace AppForSEII2526.API.Logging;

public class RabbitMQLogger : ILogger, IDisposable
{
    private readonly string _name;
    private readonly RabbitMQLoggerConfiguration _config;

    private readonly RabbitMQ.Client.IConnection _connection;
    private readonly RabbitMQ.Client.IModel _channel;
    private readonly RabbitMQ.Client.IBasicProperties _properties;

    public RabbitMQLogger(string name, RabbitMQLoggerConfiguration config)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));
        _config = config ?? throw new ArgumentNullException(nameof(config));

        ValidateConfiguration(_config);

        var factory = new RabbitMQ.Client.ConnectionFactory
        {
            HostName = _config.HostName,
            Port = _config.Port,
            UserName = _config.UserName,
            Password = _config.Password
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();


        _channel.ExchangeDeclare(
            exchange: _config.Exchange,
            type: ExchangeType.Topic,
            durable: _config.Durable);

        _properties = _channel.CreateBasicProperties();
        _properties.Persistent = true;
        _properties.ContentType = "application/json";
    }

    private static void ValidateConfiguration(RabbitMQLoggerConfiguration config)
    {
        if (string.IsNullOrWhiteSpace(config.HostName))
            throw new ArgumentException("RabbitMQ HostName is required");
        if (config.Port <= 0)
            throw new ArgumentException("RabbitMQ Port must be greater than 0");
        if (string.IsNullOrWhiteSpace(config.UserName))
            throw new ArgumentException("RabbitMQ UserName is required");
        if (string.IsNullOrWhiteSpace(config.Password))
            throw new ArgumentException("RabbitMQ Password is required");
        if (string.IsNullOrWhiteSpace(config.Exchange))
            throw new ArgumentException("RabbitMQ Exchange is required");
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default;

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        try
        {
            var logEntry = new
            {
                Timestamp = DateTime.UtcNow,
                Level = logLevel.ToString(),
                Category = _name,
                EventId = eventId.Id,
                EventName = eventId.Name,
                Message = formatter(state, exception),
                Exception = exception?.ToString()
            };

            var json = JsonSerializer.Serialize(logEntry);
            var body = Encoding.UTF8.GetBytes(json);

            string routingKey = GetRoutingKeyForLogLevel(logLevel);

            _channel.BasicPublish(
                exchange: _config.Exchange,
                routingKey: routingKey,
                basicProperties: _properties,
                body: body);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error publishing log message to RabbitMQ: {ex.Message}");
        }
    }

    private string GetRoutingKeyForLogLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Debug => "log.debug",
            LogLevel.Information => "log.info",
            LogLevel.Warning => "log.warning",
            LogLevel.Error => "log.error",
            LogLevel.Critical => "log.critical",
            _ => "log.other"
        };
    }

    public void Dispose()
    {
        try
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error disposing RabbitMQ logger: {ex.Message}");
        }

        GC.SuppressFinalize(this);
    }
}