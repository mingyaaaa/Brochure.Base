using System;
using Microsoft.Extensions.Configuration;

namespace Brochure.Base.Log.Core
{
    public class ServiceConfig
    {

        public ServiceConfig(IConfiguration configuration)
        {
            RpcHost = configuration["RpcHost:Host"];
            RpcPort = configuration.GetValue<int>("RpcHost:Port");
            DbHost = configuration["DbConnection:Host"];
            DbPort = configuration.GetValue<int>("DbConnection:Port");
            DbUser = configuration["DbConnection:User"];
            DbPassword = configuration["DbConnection:Password"];
        }
        public string RpcHost { get; }
        public int RpcPort { get; }
        public string DbHost { get; }
        public int DbPort { get; }
        public string DbUser { get; }
        public string DbPassword { get; }
    }
}
