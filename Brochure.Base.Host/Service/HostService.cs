using System;
using HostServer.Server;
using System.Collections.Generic;
using System.Linq;
namespace Brochure.Base.Host.Service
{
    public class HostService : IHostService.Iface
    {
        private IDictionary<string, List<HostConfig>> configDic = new Dictionary<string, List<HostConfig>>();
        private int index = 0;
        public HostConfig GetAddress(string serviceKey)
        {
            return GetHostConfig(serviceKey);
        }

        public void RegistAddress(string serviceKey, string host, int restPort, int rpcPort)
        {
            var config = new HostConfig(host)
            {
                RestPort = restPort,
                RpcPort = rpcPort,
                ServiceKey = serviceKey
            };
            if (configDic.ContainsKey(serviceKey))
            {
                if (configDic[serviceKey].Count > 5)
                    throw new Exception("同一个服务最多支持5个地址");
                configDic[serviceKey].Add(config);
            }
            else
                configDic[serviceKey] = new List<HostConfig>() { config };
        }
        private HostConfig GetHostConfig(string serviceKey)
        {
            var result = default(HostConfig);
            if (!configDic.ContainsKey(serviceKey))
                return result;
            var list = configDic[serviceKey];
            if (index > list.Count - 1)
                index = 0;
            result = list[index];
            index++;
            return result;
        }
    }
}
