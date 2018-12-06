using System;
namespace Brochure.Base.Log.Extends
{
    public static class LogExtends
    {
        public static LogEntity As(this LogServer.Server.Log log)
        {
            if (log == null)
                return null;
            var entity = new LogEntity();
            var time = default(DateTime);
            DateTime.TryParse(log.Time, out time);
            entity.Message = log.Message;
            entity.StackTrace = log.StackTrace;
            entity.Time = time;
            return entity;
        }
    }
}
