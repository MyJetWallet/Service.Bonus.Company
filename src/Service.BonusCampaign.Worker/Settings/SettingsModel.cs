﻿using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.BonusCampaign.Worker.Settings
{
    public class SettingsModel
    {
        [YamlProperty("BonusCampaign.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("BonusCampaign.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("BonusCampaign.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }
        
        [YamlProperty("BonusCampaign.PostgresConnectionString")]
        public string PostgresConnectionString { get; set; }        
    }
}
