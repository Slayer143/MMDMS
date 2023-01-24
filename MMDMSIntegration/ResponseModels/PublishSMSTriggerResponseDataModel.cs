using System;

namespace Terrasoft.Configuration.MMDMS.Models.ResponseModels
{
    public class PublishSMSTriggerResponseDataModel
    {
        public int id { get; set; }

        public string name { get; set; }

        public int enabled { get; set; }

        public string short_url { get; set; }
		
		public Guid uuid { get; set; }
    }
}