using System;
using System.Collections.Generic;
using Brochure.Core.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Brochure.Core;
using HostServer.Server;
using Thrift;
using Brochure.Base.Host.Service;

namespace Brochure.Base.Host
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
            var serviceManager = services.AddBrochureServer();
            var provider = serviceManager.BuildProvider();
            var boot = provider.GetService<ServerBootstrap>();
            boot.Start();
            return provider;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
            Config.HostServerAddress = Configuration["RpcHost:Host"];
            Config.HostServerPort = Configuration.GetValue<int>("RpcHost:Port");
            var hostService = new HostService();
            hostService.RegistAddress(HostServer.ServiceKey.Key, Config.HostServerAddress, -1, Config.HostServerPort);
            app.UseRpcServer(Config.HostServerPort, new Dictionary<string, TProcessor>()
            {
                [HostServer.ServiceKey.Key] = new IHostService.Processor(hostService)
            });
        }
    }
}
