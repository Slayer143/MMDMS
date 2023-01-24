namespace Terrasoft.Configuration.MMDMS.Models.ResponseModels
{
    public class ResponseDataModel<T>
    {
        public int errorCode { get; set; }
        public string errorMessage { get; set; }
		public string errorType { get; set; }
		public double executionTime { get; set; }

        public T result { get; set; }
    }
}