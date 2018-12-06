using System;
using Brochure.Core;
using LogServer.Server;
namespace Brochure.Base.Log
{
    [Table("system_log")]
    public class LogEntity : EntityBase, IBConverables<LogServer.Server.Log>
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime Time { get; set; }


        public LogServer.Server.Log Conver()
        {
            var log = new LogServer.Server.Log();
            log.Message = this.Message;
            log.StackTrace = this.StackTrace;
            log.Time = this.Time.ToString();
            return log;
        }
    }
}
