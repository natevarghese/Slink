using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Slink
{
    public static class GoogleAuthenticator
    {
        async public static Task<string> GetProfileURL(string userid)
        {
            string url = "https://www.googleapis.com/plus/v1/people/" + userid + "?fields=image&key=" + NotSensitive.SlinkKeys.google_plus_api_key;

            var client2 = new HttpClient();
            client2.Timeout = TimeSpan.FromSeconds(10);
            var contentsTask2 = await client2.GetAsync(url);
            if (contentsTask2 != null)
            {
                string result2 = contentsTask2.Content.ReadAsStringAsync().Result;
                JToken result = JToken.Parse(result2);

                if (result != null)
                {
                    var response = JsonConvert.DeserializeObject<GoogleResponse>(result["image"].ToString());

                    //fix the sizing
                    var imageUrl = response.url.Substring(0, response.url.IndexOf('?'));
                    imageUrl += "?sz=500";

                    return imageUrl;
                }
            }
            return null;
        }

    }

    public class GoogleResponse
    {
        public string url { get; set; }
    }
}
