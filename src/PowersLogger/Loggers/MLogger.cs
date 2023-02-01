using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;

namespace PowersLogger.Loggers
{
    public sealed class LoggerLevelConfiguration
    {
        public LogLevel Level
        {
            get;
            set;
        }
        
        public string? ConnectString { get; set; }
    }

    [UnsupportedOSPlatform("browser")]
    [ProviderAlias("ColorConsole")]
    public sealed  class MLoggerProvider:ILoggerProvider
    {
        private readonly IDisposable? _onChangeToken;
        private LoggerLevelConfiguration _currentConfig;

        public MLoggerProvider(
            IOptionsMonitor<LoggerLevelConfiguration> config)
        {
            _currentConfig = config.CurrentValue;
            _onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig);
        }

        public ILogger CreateLogger(string categoryName) => new MLogger(categoryName, _currentConfig);

        private LoggerLevelConfiguration GetCurrentConfig() => _currentConfig;

        public void Dispose()
        {
            _onChangeToken?.Dispose();
        }
    }

    public class MLogger:ILogger
    {
        private readonly string _categoryName;

        private readonly LoggerLevelConfiguration _config;

        public MLogger(string categoryName, LoggerLevelConfiguration config)
        {
            _categoryName = categoryName;
            _config = config;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (logLevel < _config.Level) return;
            
            Console.WriteLine($"{logLevel.ToString()}-{state!.ToString()}");
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return default!;
        }
    }
    
    public static class MLoggerExtensions
    {
        public static ILoggingBuilder AddMLogger(
            this ILoggingBuilder builder)
        {
            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<ILoggerProvider, MLoggerProvider>());

            LoggerProviderOptions.RegisterProviderOptions
                <LoggerLevelConfiguration, MLoggerProvider>(builder.Services);

            return builder;
        }

        public static ILoggingBuilder AddMLogger(
            this ILoggingBuilder builder,
            Action<LoggerLevelConfiguration> configure)
        {
            builder.AddMLogger();
            builder.Services.Configure(configure);

            return builder;
        }
    }
}
