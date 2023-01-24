using Newtonsoft.Json;
using System;

namespace Terrasoft.Configuration.MMDMS.Models.ResponseModels
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class GetBroadcastsResponseDataModel
    {
        public int id { get; set; }

        public string name { get; set; }

        public int type { get; set; }

        public string message_body { get; set; }

        public int enabled { get; set; }

        public DateTime send_date { get; set; }

        public DateTime create_date { get; set; }

        public decimal real_price { get; set; }

        public decimal estimated_price { get; set; }

        public string company_name { get; set; }

        public bool send_now { get; set; }

        public BroadcastConversion dlr { get; set; }

        public class BroadcastConversion
        {
            public int sent_count { get; set; }

            public int delivered_count { get; set; }

            public int undelivered_count { get; set; }

            public string rejected_count { get; set; }

            public string expired_count { get; set; }

            public string failed_count { get; set; }
        }
    }
}