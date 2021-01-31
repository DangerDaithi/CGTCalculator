using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace CGTCalculator.Processor.Logging
{
    public static class ApplicationLoggerFactory
    {
        private static readonly object InstanceLock = new object();        
        private static ILoggerFactory _loggerFactory = null;

        private static ILoggerFactory GetLoggerFactory()
        {
            lock (InstanceLock)
            {
                if (_loggerFactory == null)
                {
                    _loggerFactory = LoggerFactory.Create(builder => {
                            builder.AddFilter("Microsoft", LogLevel.Warning)
                                .AddFilter("System", LogLevel.Warning)
                                .AddFilter("SampleApp.Program", LogLevel.Debug)
                                .AddConsole();
                        }
                    );
                }

                return _loggerFactory;
            }
        }
        
        public static ILogger CreateLogger<T>() =>
            GetLoggerFactory().CreateLogger<T>();
    }
}