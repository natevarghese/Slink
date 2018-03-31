
using System.Net;
using Newtonsoft.Json.Linq;

namespace Tuple
{
	public class NVCRestResult
	{
		public JToken ReturnedData;
		public bool Sucessful;
		public HttpStatusCode StatusCode;

		public NVCRestResult() { }

		public NVCRestResult(JObject returnedData)
		{
			ReturnedData = returnedData;
			Sucessful = true;
		}
		public NVCRestResult(bool successful)
		{
			Sucessful = successful;
		}
	}

	public class RESTResult
	{
		public JToken ReturnedData;
		public bool Sucessful;
		public NVCRestError Error;

		public RESTResult (){}

		public RESTResult(JObject returnedData)
		{
			ReturnedData = returnedData;
			Sucessful = true;
		}
		public RESTResult(bool successful)
		{
			Sucessful = successful;
		}
	}
}