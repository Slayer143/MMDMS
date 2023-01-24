namespace Terrasoft.Configuration.MMDMS.Models.ResponseModels
{
    public class GetUTMLinksResponseDataModel : PublishUTMLinkResponseDataModel
    {
        public int id { get; set; }

        public string name { get; set; }

        public bool enabled { get; set; }

        public string url { get; set; }
    }
}