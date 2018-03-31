using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Slink
{
    public static class GithubAuthenticator
    {
        async public static Task<GithubResponse> GetGithubAccount(string token)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);

            Dictionary<string, string> Parameters = new Dictionary<string, string>();
            Parameters.Add("code", token);
            Parameters.Add("client_id", Strings.SlinkKeys.github_client_id);
            Parameters.Add("client_secret", Strings.SlinkKeys.github_client_secret);
            Parameters.Add("accept", "json");

            var contentsTask = await client.PostAsync("https://github.com/login/oauth/access_token", new FormUrlEncodedContent(Parameters));


            if (contentsTask != null)
            {//access_token=01805dc46a84d7b7547f5d430629a0354fe7ddcd&scope=user%3Aemail&token_type=bearer
                string accessToken = null;
                string result = contentsTask.Content.ReadAsStringAsync().Result;
                if (!String.IsNullOrEmpty(result))
                {
                    var splitByAmpersand = result.Split('&');
                    foreach (String sub in splitByAmpersand)
                    {
                        if (sub.Contains("access_token"))
                        {
                            var seperatedByEquals = sub.Split('=');
                            accessToken = seperatedByEquals.Last();
                        }
                    }


                    if (!String.IsNullOrEmpty(accessToken))
                    {
                        var client2 = new HttpClient();
                        client2.Timeout = TimeSpan.FromSeconds(10);
                        client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        client2.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
                        var contentsTask2 = await client2.GetAsync("https://api.github.com/user?access_token=" + accessToken);
                        if (contentsTask2 != null)
                        {
                            var result2 = JToken.Parse(contentsTask2.Content.ReadAsStringAsync().Result);
                            if (result2 != null)
                            {
                                var response = JsonConvert.DeserializeObject<GithubResponse>(result2.ToString());

                                var outlet = new Outlet();
                                outlet.Handle = response.id;
                                outlet.Type = Outlet.outlet_type_github;
                                outlet.Name = response.login;

                                RealmServices.SaveOutlet(outlet);

                                return response;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public class GithubResponse
        {
            public string id { get; set; }
            public string login { get; set; }
            public string avatar_url { get; set; }
            public string name { get; set; }
            public string url { get; set; }
        }
    }
}