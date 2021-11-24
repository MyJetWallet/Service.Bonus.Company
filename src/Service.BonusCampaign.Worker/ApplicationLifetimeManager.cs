using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.ServiceBus;
using Service.BonusCampaign.Worker.Jobs;

namespace Service.BonusCampaign.Worker
{
    public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
    {
        private readonly ILogger<ApplicationLifetimeManager> _logger;
        private readonly ServiceBusLifeTime _serviceBusLifeTime;
        private readonly CampaignCheckerJob _campaignCheckerJob;
        private readonly MyNoSqlClientLifeTime _noSqlClientLifeTime;
        public ApplicationLifetimeManager(IHostApplicationLifetime appLifetime, ILogger<ApplicationLifetimeManager> logger, ServiceBusLifeTime serviceBusLifeTime, CampaignCheckerJob campaignCheckerJob, MyNoSqlClientLifeTime noSqlClientLifeTime)
            : base(appLifetime)
        {
            _logger = logger;
            _serviceBusLifeTime = serviceBusLifeTime;
            _campaignCheckerJob = campaignCheckerJob;
            _noSqlClientLifeTime = noSqlClientLifeTime;
        }

        protected override void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");
            _serviceBusLifeTime.Start();
            _noSqlClientLifeTime.Start();
            _campaignCheckerJob.Start();
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");
            _serviceBusLifeTime.Stop();
            _noSqlClientLifeTime.Stop();
            _campaignCheckerJob.Dispose();
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");
        }
    }
}
