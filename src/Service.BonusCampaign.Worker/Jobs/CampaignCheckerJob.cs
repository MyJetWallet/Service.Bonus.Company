using Autofac;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service.Tools;
using Service.BonusCampaign.Domain.Helpers;
using Service.BonusCampaign.Domain.Models.Enums;

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
            try
            {
                var campaigns = await _campaignRepository.GetCampaigns();
                var activeCampaigns = campaigns.Where(t =>
                    t.FromDateTime <= DateTime.UtcNow && t.ToDateTime > DateTime.UtcNow && t.IsEnabled && t.Status != CampaignStatus.Active).ToList();
                foreach (var campaign in activeCampaigns)
                {
                    campaign.Status = CampaignStatus.Active;
                }

                if(activeCampaigns.Any())
                    await _campaignRepository.SetActiveCampaigns(activeCampaigns);

                var finishedCampaigns = campaigns
                    .Where(t => t.ToDateTime <= DateTime.UtcNow && t.Status != CampaignStatus.Finished).ToList();
                foreach (var campaign in finishedCampaigns)
                {
                    campaign.Status = CampaignStatus.Finished;
                }
                
                if(finishedCampaigns.Any())
                    await _campaignRepository.SetFinishedCampaigns(finishedCampaigns);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When updating campaigns statuses");
            }
        }

        public void Start()
        {
            var campaigns = _campaignRepository.GetCampaigns().GetAwaiter().GetResult();
            var activeCampaigns = campaigns.Where(t => t.FromDateTime <= DateTime.UtcNow && t.ToDateTime > DateTime.UtcNow && t.IsEnabled).ToList();
            _campaignRepository.SetActiveCampaigns(activeCampaigns).GetAwaiter().GetResult();
            _timer.Start();
        }

        public void Dispose() => _timer?.Dispose();
    }
}