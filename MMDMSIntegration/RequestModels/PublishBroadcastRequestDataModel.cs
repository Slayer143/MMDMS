using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Terrasoft.Configuration.MMDMS.Models.RequestsModels
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class PublishBroadcastRequestDataModel : PublishRequestDataModel
    {
        public int campaign_id { get; set; }

        public int broadcast_type { get; set; }

        public string sender_ids { get; set; }

        public string recipient_list_ids { get; set; }

        public string message_body { get; set; }

        public string unsubscriber_list_ids { get; set; }

        public string send_date { get; set; }

        public int utc_offset { get; set; }

        public int trigger_id { get; set; }

        public int delay { get; set; }

        public int template_id { get; set; }
    }
}