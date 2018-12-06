using System;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Infrastructure;
using Microsoft.Extensions.Logging;
namespace Brochure.Base.Log.Core
{
    public class DbLogger : ILogger
    {
        internal class Dis : IDisposable
        {
            public void Dispose()
            {

            }
        }
        IDisposable _DisposableInstance = new Dis();
        public IDisposable BeginScope<TState>(TState state)
        {
            return _DisposableInstance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            //暂时不区分日志等级
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var msg = formatter(state, exception);
            var stacktrace = exception.StackTrace;
            //实现写入日志的逻辑
        }
    }
}
