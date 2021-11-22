using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service.Tools;
using Service.BonusCampaign.Domain.Models.Enums;
using Service.BonusCampaign.Worker.Helpers;

namespace Service.BonusCampaign.Worker.Jobs
{
    public class CampaignCheckerJob : IStartable, IDisposable
    {
        private readonly CampaignRepository _campaignRepository;
        private readonly MyTaskTimer _timer;
        private readonly ILogger<CampaignCheckerJob> _logger;

        public CampaignCheckerJob(CampaignRepository campaignRepository, ILogger<CampaignCheckerJob> logger)
        {
            _campaignRepository = campaignRepository;
            _logger = logger;
            _timer = MyTaskTimer.Create<CampaignCheckerJob>(TimeSpan.FromSeconds(60), logger, DoProcess);
        }

        private async Task DoProcess()
        {
            _logger.LogInformation("Checking campaign statuses");
            var campaigns = await _campaignRepository.GetCampaigns();
            var activeCampaigns = campaigns.Where(t => t.FromDateTime <= DateTime.UtcNow && t.ToDateTime > DateTime.UtcNow && t.IsEnabled).ToList();
            foreach (var campaign in activeCampaigns)
            {
                campaign.Status = CampaignStatus.Active;
            }
            await _campaignRepository.UpsertCampaign(activeCampaigns);
            
            var finishedCampaigns = campaigns.Where(t => t.ToDateTime <= DateTime.UtcNow).ToList();
            foreach (var campaign in finishedCampaigns)
            {
                campaign.Status = CampaignStatus.Finished;
            }
            await _campaignRepository.UpsertCampaign(finishedCampaigns);
            _logger.LogInformation("Set {activeCount} campaigns as active and {finishedCount} as finished", activeCampaigns.Count, finishedCampaigns.Count);
        }

        public void Start() => _timer.Start();

        public void Dispose() => _timer?.Dispose();
    }
}