using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Slink
{
    public static class TwilioServices
    {
        public static string twilio_api_key = "A0vfEnETd3dq7zs3PJn9KDZ7olWm7kp6";

        public static async Task<TwilioStartVerificationResponse> StartPhoneVerificationAsync(string phoneNumber)
        {
            TwilioStartVerificationResponse twilioResponse = null;

            // Create client
            var client = new HttpClient();

            var requestContent = new FormUrlEncodedContent(new[] {
              new KeyValuePair<string, string>("via", "sms"),
              new KeyValuePair<string, string>("phone_number", phoneNumber),
              new KeyValuePair<string, string>("country_code", "1"),
            });

            HttpResponseMessage response = await client.PostAsync("https://api.authy.com/protected/json/phones/verification/start?api_key=" + twilio_api_key, requestContent);
            HttpContent responseContent = response.Content;

            // Get the stream of the content.
            using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
            {
                var content = await reader.ReadToEndAsync();
                twilioResponse = JsonConvert.DeserializeObject<TwilioStartVerificationResponse>(content);
            }

            return twilioResponse;
        }


        public static async Task<TwilioVerificationResponse> VerifyPhoneAsync(string phoneNumber, string code)
        {
            TwilioVerificationResponse twilioResponse = null;

            // Create client
            var client = new HttpClient();

            // Add authentication header
            client.DefaultRequestHeaders.Add("X-Authy-API-Key", twilio_api_key);

            HttpResponseMessage response = await client.GetAsync("https://api.authy.com/protected/json/phones/verification/check?phone_number=" + phoneNumber + "&country_code=1&verification_code=" + code);

            // Get the response content.
            HttpContent responseContent = response.Content;

            // Get the stream of the content.
            using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
            {
                var content = await reader.ReadToEndAsync();
                twilioResponse = JsonConvert.DeserializeObject<TwilioVerificationResponse>(content);
            }

            return twilioResponse;
        }
    }

    public class TwilioVerificationResponse
    {
        public string message { get; set; }
        public bool success { get; set; }
    }
    public class TwilioStartVerificationResponse
    {
        public string carrier { get; set; }
        public bool is_cellphone { get; set; }
        public string message { get; set; }
        public int seconds_to_expire { get; set; }
        public string uuid { get; set; }
        public bool success { get; set; }
    }
}

