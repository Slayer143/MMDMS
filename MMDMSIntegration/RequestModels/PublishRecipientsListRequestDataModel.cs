using System.Collections.Generic;

namespace Terrasoft.Configuration.MMDMS.Models.RequestsModels
{
    public class PublishRecipientsListRequestDataModel : PublishRequestDataModel
    {
        public List<string> numbers { get; set; }
    }
}