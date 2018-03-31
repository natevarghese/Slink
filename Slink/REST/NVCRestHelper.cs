using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace Slink
{
    public static class NVCRestHelper
    {
        static HttpClient Client = new HttpClient();
        static MediaTypeWithQualityHeaderValue MediaTypeWithQualityHeaderValueJson = new MediaTypeWithQualityHeaderValue("application/json");

        public static HttpClient GetHttpClient(NVCEndpoint endpoint, List<string> appendString)
        {
            var iPersistantStorage = ServiceLocator.Instance.Resolve<IPersistantStorage>();
            var facebookToken = iPersistantStorage.GetFacebookToken();

            Client.Timeout = TimeSpan.FromSeconds(Math.Max(endpoint.TimeoutSections, 20));
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", facebookToken);

            if (!Client.DefaultRequestHeaders.Accept.Contains(MediaTypeWithQualityHeaderValueJson))
                Client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValueJson);

            StringBuilder stringBuilder = new StringBuilder(Strings.SystemUrls.base_url + endpoint.Controller + endpoint.URL);
            if (appendString != null && appendString.Count > 0)
            {
                stringBuilder.Append("/");

                foreach (String str in appendString)
                {
                    stringBuilder.Append(str);
                    stringBuilder.Append("/");
                }
            }

            Client.BaseAddress = new Uri(stringBuilder.ToString());

            return Client;
        }

        public static async Task<NVCRestResult> Async(NVCEndpoint endpoint, Dictionary<string, object> parameters, List<string> appendString)
        {
            HttpResponseMessage contentsTask = null;
            try
            {
                switch (endpoint.Method)
                {
                    case NVCEndpoint.EndpointType.Post:
                        contentsTask = await Post(endpoint, parameters, appendString);
                        break;
                    case NVCEndpoint.EndpointType.PostXWWWFormUrlEncoded:
                        contentsTask = await PostXWWWFormUrlEncoded(endpoint, parameters, appendString);
                        break;
                    case NVCEndpoint.EndpointType.PostRaw:
                        contentsTask = await PostRaw(endpoint, parameters, appendString);
                        break;
                    case NVCEndpoint.EndpointType.Get:
                        contentsTask = await Get(endpoint, parameters, appendString);
                        break;
                    case NVCEndpoint.EndpointType.Put:
                        contentsTask = await Put(endpoint, parameters, appendString);
                        break;
                    default:
                        break;
                }

                if (contentsTask == null) return new NVCRestResult(false);

                NVCRestResult restResult = new NVCRestResult(contentsTask.IsSuccessStatusCode);
                restResult.StatusCode = contentsTask.StatusCode;

                if (restResult.Sucessful)
                {
                    string result = contentsTask.Content.ReadAsStringAsync().Result;
                    if (!String.IsNullOrEmpty(result))
                        restResult.ReturnedData = JObject.Parse(contentsTask.Content.ReadAsStringAsync().Result);
                }

                return restResult;

            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception.Message);
                throw exception;
            }
        }



        //GET
        public static async Task<HttpResponseMessage> Get(NVCEndpoint endpoint, Dictionary<string, object> Parameters, List<String> appendString)
        {
            var client = GetHttpClient(endpoint, appendString);

            if (Parameters != null)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (KeyValuePair<String, object> entry in Parameters)
                {
                    stringBuilder.Append(entry.Key);
                    stringBuilder.Append("=");
                    stringBuilder.Append(entry.Value);
                    stringBuilder.Append("&");
                }

                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Remove(stringBuilder.Length - 1, 1);
                    client.BaseAddress = new Uri(client.BaseAddress + "?" + stringBuilder);
                }
            }

            return await client.GetAsync(endpoint.URL);
        }
        //PUT
        public static async Task<HttpResponseMessage> Put(NVCEndpoint endpoint, Dictionary<string, object> Parameters, List<String> appendString)
        {
            var client = GetHttpClient(endpoint, appendString);
            return await client.PutAsync(endpoint.URL, new StringContent(JsonConvert.SerializeObject(Parameters), Encoding.UTF8, "application/json"));
        }

        //POST
        public static async Task<HttpResponseMessage> Post(NVCEndpoint endpoint, Dictionary<string, object> Parameters, List<String> appendString)
        {
            var client = GetHttpClient(endpoint, appendString);

            string contentString = String.Empty;
            HttpContent contentHttp = null;

            if (Parameters != null)
            {
                contentString = JsonConvert.SerializeObject(Parameters).ToString();
                contentHttp = new StringContent(contentString, Encoding.UTF8, "application/json");
            }

            return await client.PostAsync(endpoint.URL, contentHttp);
        }
        public static async Task<HttpResponseMessage> PostRaw(NVCEndpoint endpoint, Dictionary<string, object> Parameters, List<String> appendString)
        {
            var client = GetHttpClient(endpoint, appendString);
            if (Parameters == null)
                return await client.PostAsync(endpoint.URL, null);

            var content = new StringContent(JsonConvert.SerializeObject(Parameters), Encoding.UTF8, "application/json");
            return await client.PostAsync(endpoint.URL, content);
        }
        public static async Task<HttpResponseMessage> PostFormData(NVCEndpoint endpoint, Dictionary<string, object> Parameters, List<String> appendString)
        {
            var client = GetHttpClient(endpoint, appendString);
            MultipartFormDataContent content = new MultipartFormDataContent("boundary=---------------------------14737809831466499882746641449");
            foreach (KeyValuePair<String, object> entry in Parameters)
            {
                if (entry.Value is string)
                {
                    string key = entry.Key;
                    string val = entry.Value.ToString();
                    content.Add(new StringContent(val), key);
                }
                else
                {
                    var imageContent = new ByteArrayContent((byte[])entry.Value);
                    imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                    content.Add(imageContent, "file", "image.jpg");
                }
            }
            return await client.PostAsync(endpoint.URL, content);

        }

        public static async Task<HttpResponseMessage> PostXWWWFormUrlEncoded(NVCEndpoint endpoint, Dictionary<string, object> Parameters, List<String> appendString)
        {
            var client = GetHttpClient(endpoint, appendString);
            Dictionary<String, String> dictToSend = new Dictionary<string, string>();
            foreach (KeyValuePair<string, object> entry in Parameters)
            {
                if (entry.Value != null)
                    dictToSend.Add(entry.Key, entry.Value.ToString());
            }
            return await client.PostAsync(endpoint.URL, new FormUrlEncodedContent(dictToSend));
        }

    }
}
