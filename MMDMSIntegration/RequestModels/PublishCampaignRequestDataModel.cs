using Newtonsoft.Json;
using System;

namespace Terrasoft.Configuration.MMDMS.Models.RequestsModels
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class PublishCampaignRequestDataModel
    {
        public string name { get; set; }
		
		public DateTime? start_date { get; set; }
		
		public DateTime? end_date { get; set; }

        public PublishCampaignRequestDataModel(string name, DateTime? startDate, DateTime? endDate)
        {
            this.name = name;

            if (startDate.HasValue && startDate.Value == DateTime.MinValue)
                start_date = startDate.Value;

            if (endDate.HasValue && startDate.Value == DateTime.MinValue && endDate > startDate)
                end_date = endDate.Value;
        }

        public PublishCampaignRequestDataModel() { }
    }
}