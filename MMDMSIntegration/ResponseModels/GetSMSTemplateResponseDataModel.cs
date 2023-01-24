 namespace Terrasoft.Configuration.MMDMS.Models.ResponseModels
{
    public class GetSMSTemplateResponseDataModel
    {
        public int id { get; set; }

        public string name { get; set; }

        public bool enabled { get; set; }

        public string body { get; set; }
    }
}