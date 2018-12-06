using LogServer.Server;
using System.Collections.Generic;
using Brochure.Base.Log;
using Brochure.Core.Core;
using Brochure.Core.Server;
using System.Threading.Tasks;
using System.Linq;
using Brochure.Base.Log.Extends;
using System.Timers;
using System;

namespace Brochure.Base.Logs.Service
{
    public class LogService : ILogService.Iface
    {
        private IClient _dbClient;
        private Timer _timer;
        public LogService(IClient client)
        {
            _dbClient = client;
            _timer = new Timer(2000);
            _timer.AutoReset = true;
            _timer.Elapsed += (object sender, ElapsedEventArgs e) => WriteToDbAsync();
        }
        private static Stack<LogEntity> LogServerCache = new Stack<LogEntity>();

        public void Error(LogServer.Server.Log log)
        {
            var entity = log.As();
            entity.Type = "Error";
            entity.Time = DateTime.Now;
            LogServerCache.Push(entity);
        }

        public void Info(LogServer.Server.Log log)
        {
            var entity = log.As();
            entity.Type = "Info";
            entity.Time = DateTime.Now;
            LogServerCache.Push(entity);
        }

        public void Warning(LogServer.Server.Log log)
        {
            var entity = log.As();
            entity.Type = "Warning";
            entity.Time = DateTime.Now;
            LogServerCache.Push(entity);
        }
        private async void WriteToDbAsync()
        {
            var logHub = await _dbClient.GetDataHubAsync<LogEntity>();
            await Task.Run(() =>
             {
                 while (LogServerCache.Count > 0)
                 {
                     var entity = LogServerCache.Pop();
                     logHub.InserOneAsync(entity);
                 }
             });
        }
    }
}
