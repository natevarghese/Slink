using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Net;

namespace Tuple
{
	public class NVCRestError : Exception
	{

	}
		
	public class RESTError : Exception
	{
		public int StatusCode;
		public string RestMessage;
		public string RestStackTrace;

		public string URL;
		public Endpoint.EndpointType EndpointType;
		public Dictionary<String, object> Parameters;

		public Exception Exception;

		public RESTError() { }
		public RESTError(WebException e)
		{
			StatusCode = 0;
			URL = null;
			RestMessage = e.Message;
			RestStackTrace = e.StackTrace;
			EndpointType = Endpoint.EndpointType.Fake;
			Parameters = null;
			Exception = null;

			System.Diagnostics.Debug.WriteLine(Message);
		}
		public RESTError(Exception e)
		{
			StatusCode = 0;
			RestMessage = e.Message;
			RestStackTrace = e.StackTrace;
			URL = null;
			EndpointType = Endpoint.EndpointType.Fake;
			Parameters = null;
			Exception = null;
		}

		public RESTError(string message)
		{
			StatusCode = 0;
			RestMessage = message;
			RestStackTrace = null;
			URL = null;
			EndpointType = Endpoint.EndpointType.Fake;
			Parameters = null;
			Exception = null;
		}
		public RESTError(int statusCode, JToken token, string endpoint, Endpoint.EndpointType type, Dictionary<String, object> parameters)
		{
			StatusCode = statusCode;
			RestMessage = (string)token["message"];
			RestStackTrace = (string)token["StackTrace"];
			URL = endpoint;
			EndpointType = type;
			Parameters = parameters;
			Exception = null;
		}

		public RESTError(int statusCode, string message, string stackTrace, string endpoint, Endpoint.EndpointType type, Dictionary<String, object> parameters)
		{
			StatusCode = statusCode;
			RestMessage = message;
			RestStackTrace = stackTrace;
			URL = endpoint;
			EndpointType = type;
			Parameters = parameters;
			Exception = null;
		}

		public RESTError(string endpoint, Endpoint.EndpointType type, Dictionary<String, object> parameters, Exception exception)
		{
			if (exception != null)
			{
				RestMessage = exception.Message;
				RestStackTrace = exception.StackTrace;
			}
			StatusCode = 0;
			URL = endpoint;
			EndpointType = type;
			Parameters = parameters;
			Exception = exception;
		}
	}
}
