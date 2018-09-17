using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Auth;

namespace Slink
{
    public static class TwitterAuthenticator
    {
        public static OAuth1Authenticator GetTwitterAuthenticator(Action<bool> sucessful)
        {
            var auth = new OAuth1Authenticator(NotSensitive.SlinkKeys.twitter_consumer_key, NotSensitive.SlinkKeys.twitter_consumer_secret,
                  new Uri("https://api.twitter.com/oauth/request_token"),
                  new Uri("https://api.twitter.com/oauth/authorize"),
                    new Uri("https://api.twitter.com/oauth/access_token"), new Uri("https://mobile.twitter.com/home"));

            auth.ShowErrors = false;

            auth.Completed += async (s, eventArgs) =>
            {
                if (!eventArgs.IsAuthenticated) return;

                var loggedInAccount = eventArgs.Account;

                TwitterResponse results = null;
                if (loggedInAccount.Properties.Count >= 4)
                    results = await TwitterAuthenticator.GetTwitterAccount(loggedInAccount);

                sucessful?.Invoke(results != null);
            };

            return auth;
        }
        async public static Task<TwitterResponse> GetTwitterAccount(Account account)
        {
            string credentials_url = "https://api.twitter.com/1.1/account/verify_credentials.json";
            var request = new OAuth1Request("GET", new Uri(credentials_url), null, account);

            TwitterResponse twitterResponse = null;

            await request.GetResponseAsync().ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    System.Diagnostics.Debug.WriteLine("Error: " + t.Exception.InnerException.Message);
                    return;
                }

                var res = t.Result;
                var resString = res.GetResponseText();

                twitterResponse = JsonConvert.DeserializeObject<TwitterResponse>(resString);

                var outlet = new Outlet();
                outlet.Handle = twitterResponse.screen_name;
                outlet.Type = Outlet.outlet_type_twitter;
                outlet.Name = twitterResponse.screen_name;

                RealmServices.SaveOutlet(outlet);
            });

            return twitterResponse;
        }


    }

    public class TwitterResponse
    {
        public int id { get; set; }
        public string id_str { get; set; }
        public string name { get; set; }
        public string screen_name { get; set; }
    }
}