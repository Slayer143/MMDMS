using System;

namespace Terrasoft.Configuration.MMDMS.Models.ResponseModels
{
    public class PublishCampaignResponseDataModel
    {
        public int id { get; set; }

        public int is_default { get; set; }

        public string name { get; set; }

        public DateTime? start_date { get; set; }

        public DateTime? end_date { get; set; }

        public bool enabled { get; set; }
    }
}