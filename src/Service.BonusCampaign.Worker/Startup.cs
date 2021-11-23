using System.Reflection;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyJetWallet.Sdk.GrpcSchema;
using MyJetWallet.Sdk.Postgres;
using MyJetWallet.Sdk.Service;
using Prometheus;
using Service.BonusCampaign.Postgres;
using Service.BonusCampaign.Worker.Modules;
using SimpleTrading.ServiceStatusReporterConnector;

namespace Service.BonusCampaign.Worker
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureJetWallet<ApplicationLifetimeManager>(Program.Settings.ZipkinUrl);
            DatabaseContext.LoggerFactory = Program.LogFactory;
            services.AddDatabase(DatabaseContext.Schema, Program.Settings.PostgresConnectionString.Replace("Ssl Mode=Require", "Ssl Mode=VerifyFull"),
                o => new DatabaseContext(o));
            DatabaseContext.LoggerFactory = null;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ConfigureJetWallet(env, endpoints =>
            {
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.ConfigureJetWallet();
            builder.RegisterModule<SettingsModule>();
            builder.RegisterModule<ServiceModule>();
        }
    }
}
