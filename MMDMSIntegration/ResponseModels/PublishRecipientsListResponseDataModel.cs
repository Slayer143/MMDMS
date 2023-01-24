namespace Terrasoft.Configuration.MMDMS.Models.ResponseModels
{
    public class PublishRecipientsListResponseDataModel
    {
        public int inserted { get; set; }
        public int wrong { get; set; }
        public int duplicate { get; set; }
        public int total { get; set; }
        public int recipient_list_id { get; set; }
        public int count { get; set; }
    }
}