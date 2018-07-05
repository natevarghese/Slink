using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Slink
{
    public static class InstagramAuthenticator
    {
        async public static Task<InstagramResponse> GetInstagramAccount(string token)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);

            Dictionary<string, string> Parameters = new Dictionary<string, string>();
            Parameters.Add("grant_type", "authorization_code");
            Parameters.Add("code", token);
            Parameters.Add("redirect_uri", NotSensitive.SystemUrls.instagram_redirect_url);
            Parameters.Add("client_id", NotSensitive.SlinkKeys.instagram_client_id);
            Parameters.Add("client_secret", NotSensitive.SlinkKeys.instagram_client_secret);


            var contentsTask = await client.PostAsync("https://api.instagram.com/oauth/access_token", new FormUrlEncodedContent(Parameters));


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
                        var contentsTask2 = await client2.GetAsync("https://api.instagram.com/v1/users/self/?access_token=" + accessToken);
                        if (contentsTask2 != null)
                        {
                            var result2 = JToken.Parse(contentsTask2.Content.ReadAsStringAsync().Result);
                            if (result2 != null)
                            {
                                var response = JsonConvert.DeserializeObject<InstagramResponse>(result2["data"].ToString());

                                var outlet = new Outlet();
                                outlet.Handle = response.username;
                                outlet.Type = Outlet.outlet_type_instagram;
                                outlet.Name = response.full_name;

                                RealmServices.SaveOutlet(outlet);

                                return response;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public class InstagramResponse
        {
            public string id { get; set; }
            public string username { get; set; }
            public string profile_picture { get; set; }
            public string full_name { get; set; }
            public string bio { get; set; }
            public string website { get; set; }
        }
    }
}