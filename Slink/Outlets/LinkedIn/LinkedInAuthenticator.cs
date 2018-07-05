using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Linq;

namespace Slink
{
    public static class LinkedInAuthenticator
    {
        async public static Task<LinkedInResponse> GetLinkedInAccount(string token)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);

            Dictionary<string, string> Parameters = new Dictionary<string, string>();
            Parameters.Add("grant_type", "authorization_code");
            Parameters.Add("code", token);
            Parameters.Add("redirect_uri", "https://mobile.linkedin.com");
            Parameters.Add("client_id", NotSensitive.SlinkKeys.linkedin_client_id);
            Parameters.Add("client_secret", NotSensitive.SlinkKeys.linkedin_client_secret);


            var contentsTask = await client.PostAsync("https://www.linkedin.com/uas/oauth2/accessToken", new FormUrlEncodedContent(Parameters));


            if (contentsTask != null)
            {
                string result = contentsTask.Content.ReadAsStringAsync().Result;
                if (!String.IsNullOrEmpty(result))
                {
                    JToken jToken = JToken.Parse(result);
                    if (jToken == null || jToken["access_token"] == null) return null;

                    var accessToken = jToken["access_token"].ToString();
                    if (!String.IsNullOrEmpty(accessToken))
                    {
                        var client2 = new HttpClient();
                        client2.Timeout = TimeSpan.FromSeconds(10);
                        client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        var contentsTask2 = await client2.GetAsync("https://api.linkedin.com/v1/people/~:(id,first-name,last-name,public-profile-url)?format=json");
                        if (contentsTask2 != null)
                        {
                            string result2 = contentsTask2.Content.ReadAsStringAsync().Result;
                            if (!String.IsNullOrEmpty(result2))
                            {
                                var response = JsonConvert.DeserializeObject<LinkedInResponse>(result2);

                                var handle = response.publicProfileUrl.Split('/').Last();

                                var outlet = new Outlet();
                                outlet.Handle = handle;
                                outlet.Type = Outlet.outlet_type_linkedIn;
                                outlet.Name = response.firstName + " " + response.lastName;

                                RealmServices.SaveOutlet(outlet);

                                return response;
                            }
                        }
                    }
                }
            }

            return null;
        }
    }

    public class LinkedInResponse
    {
        public string id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string publicProfileUrl { get; set; }
    }
}
