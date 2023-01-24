 using System.Collections.Generic;

namespace Terrasoft.Configuration.MMDMS.Models.RequestsModels
{
    public class PublishUnsubscribeListRequestDataModel : PublishRequestDataModel
    {
        public List<string> numbers { get; set; }
    }
}