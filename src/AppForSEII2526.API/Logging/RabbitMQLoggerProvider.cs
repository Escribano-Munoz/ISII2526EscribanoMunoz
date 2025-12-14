using AppForSEII2526.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace AppForSEII2526.API.Logging;

[ProviderAlias("RabbitMQ")]
public class RabbitMQLoggerProvider : ILoggerProvider
{
    private readonly RabbitMQLoggerConfiguration _config;
    private readonly ConcurrentDictionary<string, RabbitMQLogger> _loggers = new();

    public RabbitMQLoggerProvider(IOptions<RabbitMQLoggerConfiguration> config)
    {
        _config = config.Value;
    }

    public ILogger CreateLogger(string categoryName)
        => _loggers.GetOrAdd(categoryName, name => new RabbitMQLogger(name, _config));

    public void Dispose()
    {
        foreach (var logger in _loggers.Values)
            logger.Dispose();

        _loggers.Clear();
    }
}