using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace SRCCore.Logging.Tests
{
    /// <summary>出されたログを記録する ILoggerProvider</summary>
    internal sealed class RecordingLoggerProvider : ILoggerProvider
    {
        public List<(string Category, LogLevel Level, string Message, Exception Exception)> Entries { get; } = new List<(string, LogLevel, string, Exception)>();

        public ILogger CreateLogger(string categoryName) => new Logger(this, categoryName);

        public void Dispose()
        {
        }

        private sealed class Logger : ILogger
        {
            private readonly RecordingLoggerProvider provider;
            private readonly string category;

            public Logger(RecordingLoggerProvider provider, string category)
            {
                this.provider = provider;
                this.category = category;
            }

            public IDisposable BeginScope<TState>(TState state) => null;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                lock (provider.Entries)
                {
                    provider.Entries.Add((category, logLevel, formatter(state, exception), exception));
                }
            }
        }
    }
}
