using Brochure.Base.Log.Core;
using Brochure.Base.Logs.Service;
using Brochure.Core;
using Brochure.Core.Server;
using Brochure.Server.MySql;
using Brochure.Server.MySql.Implements;
using HostServer.Server;
using LogServer.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Thrift;

namespace Brochure.Base.Log
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            Config.HostServerAddress = Configuration["HostService:Host"];
            Config.HostServerPort = Configuration.GetValue<int>("HostService:Port");

            var serviceManager = services.AddBrochureServer();
            var config = new ServiceConfig(Configuration);

            DbFactory factory = new MySqlDbFactory(config.DbHost, config.DbUser, config.DbPassword, config.DbPort.ToString());
            DbConnectPool.RegistFactory(factory, DatabaseType.MySql);
            serviceManager.AddSingleton(new RpcClient<IHostService.Client>(LogServer.ServiceKey.Key));
            serviceManager.AddSingleton(config);
            serviceManager.AddSingleton<IClient, MySqlClient>();
            serviceManager.AddSingleton(typeof(LogService));
            var provider = serviceManager.BuildProvider();
            var boot = provider.GetService<ServerBootstrap>();
            boot.Start();
            return provider;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

            var serviceConfig = provider.GetService<ServiceConfig>();

            var hostClient = provider.GetService<RpcClient<IHostService.Client>>();
            hostClient.Client.RegistAddress(LogServer.ServiceKey.Key, serviceConfig.RpcHost, -1, serviceConfig.RpcPort);
            var logService = provider.GetService<LogService>();
            app.UseRpcServer(serviceConfig.RpcPort, new Dictionary<string, TProcessor>()
            {
                [LogServer.ServiceKey.Key] = new ILogService.Processor(logService)
            });
            //初始化数据库
            var dbClient = provider.GetService<IClient>();
            var databaseHub = await dbClient.GetDatabaseHubAsync();
            var databaseName = "LogServerDb";
            await databaseHub.CreateDataBaseAsync(databaseName);
            dbClient.DatabaseName = databaseName;
            //创建表
            var tableHub = await dbClient.GetDataTableHubAsync();
            await tableHub.CreateTableAsync<LogEntity>();
        }
    }
}
