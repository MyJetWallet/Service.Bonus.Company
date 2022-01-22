using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MyJetWallet.DynamicLinkGenerator.NoSql;
using MyJetWallet.DynamicLinkGenerator.Services;
using MyNoSqlServer.DataReader;
using ProtoBuf.Grpc.Client;
using Service.BonusCampaign.Client;
using Service.BonusCampaign.Domain.Models.NoSql;
using Service.BonusCampaign.Grpc.Models;
using Service.MessageTemplates.Client;
using Service.MessageTemplates.Domain.Models.NoSql;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var clientId = "62817710c2794afb9648e28b6203161d";
            GrpcClientFactory.AllowUnencryptedHttp2 = true;
            var myNoSqlClient = new MyNoSqlTcpClient(() => "192.168.70.80:5125", "BonusCampaignTestApp");
            
            var templatesClientFactory = new MessageTemplatesClientFactory("http://messagetemplates.spot-services.svc.cluster.local:80");
            var templateService = templatesClientFactory.GetTemplateService();
            var templateSubs = new MyNoSqlReadRepository<TemplateNoSqlEntity>(myNoSqlClient, TemplateNoSqlEntity.TableName);
            var templateClient = new TemplateClient(templateSubs, templateService);
            
            var linkSubs = new MyNoSqlReadRepository<DynamicLinkSettingsNoSql>(myNoSqlClient, DynamicLinkSettingsNoSql.TableName);
            var dynamicLinkClient = new DynamicLinkClient(linkSubs);
            var contextSubs = new MyNoSqlReadRepository<CampaignClientContextNoSqlEntity>(myNoSqlClient, CampaignClientContextNoSqlEntity.TableName);
            var campaignSubs = new MyNoSqlReadRepository<CampaignNoSqlEntity>(myNoSqlClient, CampaignNoSqlEntity.TableName);
            
            var statFactory = new BonusCampaignClientFactory("http://bonuscampaign.spot-services.svc.cluster.local:80",
                contextSubs, campaignSubs, templateClient, dynamicLinkClient);
            
            myNoSqlClient.Start();

            while (campaignSubs.Count() == 0)
            {
                Thread.Sleep(1000);
            }
            
            var stats =  statFactory.GetCampaignStatService();
            var contextService = statFactory.GetClientContextService();

            while (true)
            {
                var campaigns = campaignSubs.Get().Select(t=>t.Campaign).ToList();
                var contexts = await contextService.GetContextsByClient(new GetContextsByClientRequest
                {
                    ClientId = clientId
                });
                var campaignsStats = await stats.GetCampaignsStats(new CampaignStatRequest
                {
                    ClientId = clientId,
                    Brand = "simple",
                    Lang = "En",
                });

                //Console.WriteLine(JsonSerializer.Serialize(campaigns));
                //Console.WriteLine(JsonSerializer.Serialize(contexts));
                Console.WriteLine(JsonSerializer.Serialize(campaignsStats));

                Console.WriteLine("End");
                Console.ReadLine();
            }
        }
    }
}
