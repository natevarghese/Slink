using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Slink
{
    public static class PinterestAuthenticator
    {
        async public static Task<PinterestResponse> GetPinterestAccount(string token)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);

            Dictionary<string, string> Parameters = new Dictionary<string, string>();
            Parameters.Add("grant_type", "authorization_code");
            Parameters.Add("code", token);
            Parameters.Add("redirect_uri", Strings.SystemUrls.pinterest_redirect_url);
            Parameters.Add("client_id", Strings.SlinkKeys.pinterest_client_id);
            Parameters.Add("client_secret", Strings.SlinkKeys.pinterest_client_secret);


            var contentsTask = await client.PostAsync("https://api.pinterest.com/v1/oauth/token", new FormUrlEncodedContent(Parameters));


            if (contentsTask != null)
            {
                string result = contentsTask.Content.ReadAsStringAsync().Result;
                if (!String.IsNullOrEmpty(result))
                {
                    JToken jToken = JToken.Parse(result);
                    var accessToken = jToken["access_token"].ToString();
                    if (!String.IsNullOrEmpty(accessToken))
                    {
                        var client2 = new HttpClient();
                        client2.Timeout = TimeSpan.FromSeconds(10);
                        var contentsTask2 = await client2.GetAsync("https://api.pinterest.com/v1/me/?access_token=" + accessToken);
                        if (contentsTask2 != null)
                        {
                            var result2 = JToken.Parse(contentsTask2.Content.ReadAsStringAsync().Result);
                            if (result2 != null)
                            {
                                var response = JsonConvert.DeserializeObject<PinterestResponse>(result2["data"].ToString());

                                var outlet = new Outlet();
                                outlet.Handle = response.id;
                                outlet.Type = Outlet.outlet_type_pinterest;
                                outlet.Name = response.first_name + response.last_name;

                                RealmServices.SaveOutlet(outlet);

                                return response;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public class PinterestResponse
        {
            public string id { get; set; }
            public string last_name { get; set; }
            public string first_name { get; set; }
            public string url { get; set; }
        }
    }
}