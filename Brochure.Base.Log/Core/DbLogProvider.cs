using System;
using Microsoft.Extensions.Logging;
namespace Brochure.Base.Log.Core
{
    public class DbLogProvider : ILoggerProvider
    {
        public DbLogProvider()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new DbLogger();
        }

        public void Dispose()
        {
        }
    }
}
