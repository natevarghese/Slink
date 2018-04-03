using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Threading;
using Newtonsoft.Json.Linq;
using Amazon.S3.Model;

namespace Slink
{
    public static class WebServices
    {

        public static class UserController
        {
            public static string Mapping = "user";

            public static async Task<NVCRestResult> CreateUser()
            {
                NVCEndpoint endpoint = new NVCEndpoint();
                endpoint.Method = NVCEndpoint.EndpointType.PostRaw;
                endpoint.Controller = Mapping;

                return await NVCRestHelper.Async(endpoint, new Dictionary<string, object>(), null);
            }

            public static async Task UpdateUser(string oneSignalId)
            {
                Dictionary<string, object> requestDictionary = new Dictionary<string, object>();
                requestDictionary.Add("oneSignalId", oneSignalId);

                NVCEndpoint endpoint = new NVCEndpoint();
                endpoint.Method = NVCEndpoint.EndpointType.Put;
                endpoint.Controller = Mapping;

                await NVCRestHelper.Async(endpoint, requestDictionary, null);
            }
            public static async Task UpdateUser(double lat, double lng)
            {
                Dictionary<string, object> requestDictionary = new Dictionary<string, object>();
                requestDictionary.Add("lat", lat);
                requestDictionary.Add("lng", lng);

                NVCEndpoint endpoint = new NVCEndpoint();
                endpoint.Method = NVCEndpoint.EndpointType.Put;
                endpoint.Controller = Mapping;

                await NVCRestHelper.Async(endpoint, requestDictionary, null);

                System.Diagnostics.Debug.WriteLine("User location updated on server: lat: " + lat + " lon: " + lng);
            }

        }
        public static class TransactionsController
        {
            public static string Mapping = "transactions";

            public static async Task<string> CreateTransaction(double lat, double lng, Card card, string userName)
            {
                //replace the card name w/ the users name
                var obj = card.ToJObject();
                obj["name"] = userName;
                obj["uuid"] = Guid.NewGuid().ToString();

                Dictionary<string, object> requestDictionary = new Dictionary<string, object>();
                requestDictionary.Add("lat", lat);
                requestDictionary.Add("lng", lng);
                requestDictionary.Add("distance", 1);
                requestDictionary.Add("card", obj);

                NVCEndpoint endpoint = new NVCEndpoint();
                endpoint.Method = NVCEndpoint.EndpointType.PostRaw;
                endpoint.Controller = Mapping;

                var data = await NVCRestHelper.Async(endpoint, requestDictionary, null);
                if (!data.Sucessful) return null;

                var token = data.ReturnedData;
                if (token["id"] == null) return null;


                var transactionId = (string)token["id"];
                return transactionId;
            }

            public static async Task TerminateTransaction(string transactionId)
            {
                List<string> appendString = new List<string>() { transactionId, "terminate" };

                NVCEndpoint endpoint = new NVCEndpoint();
                endpoint.Method = NVCEndpoint.EndpointType.PostRaw;
                endpoint.Controller = Mapping;

                await NVCRestHelper.Async(endpoint, null, appendString);
            }
            public static async Task GetTranactions(List<Card> itemsToIgnore)
            {
                NVCEndpoint endpoint = new NVCEndpoint();
                endpoint.Method = NVCEndpoint.EndpointType.Get;
                endpoint.Controller = Mapping;

                var data = await NVCRestHelper.Async(endpoint, new Dictionary<string, object>(), null);
                if (!data.Sucessful) return;

                var token = data.ReturnedData;
                if (token == null) return;

                var transactions = token["transactions"] as JArray;
                if (transactions == null) return;

                foreach (var transactionToken in transactions)
                    ParseTransaction(transactionToken, itemsToIgnore);
            }

            static ExchangeTransaction ParseTransaction(JToken token, List<Card> itemsToIgnore)
            {
                if (token == null) return null;
                if (token["id"] == null) return null;
                if (token["card"] == null) return null;
                if (token["lat"] == null) return null;
                if (token["lng"] == null) return null;

                var cardToken = token["card"];
                if (cardToken == null) return null;

                //check if we should ignore it
                var uuid = (string)cardToken["uuid"];
                if (itemsToIgnore.Any(c => c.UUID.Equals(uuid, StringComparison.OrdinalIgnoreCase))) return null;


                string id = (string)token["id"];
                if (String.IsNullOrEmpty(id)) return null;

                ExchangeTransaction transaction = new ExchangeTransaction();
                transaction.UUID = id;
                transaction.Latitidue = (double)token["lat"];
                transaction.Longitude = (double)token["lng"];
                transaction.AccessToken = (string)cardToken["accessKey"];

                var realm = RealmManager.SharedInstance.GetRealm(null);
                realm.Write(() =>
                {
                    var user = new SlinkUser();
                    user.ID = (string)token["facebookId"];
                    user.FacebookID = (string)token["facebookId"];
                    user.Name = (string)cardToken["name"];
                    realm.Add(user, true);

                    //fetch fresh copy 
                    //this is needed in case you get a user already with one card in in Connections
                    user = realm.All<SlinkUser>().Where(c => c.ID.Equals(user.ID, StringComparison.OrdinalIgnoreCase)).First();
                    var before = user.Cards.Count();

                    var card = new Card();
                    card.UUID = uuid;
                    card.Name = (string)cardToken["name"];
                    card.UserDisplayName = (string)cardToken["userDisplayName"];
                    card.Title = (string)cardToken["title"];
                    card.BorderColor = (string)cardToken["borderColor"];
                    card.BackgroundColor = (string)cardToken["backgroundColor"];
                    card.CompanyName = (string)cardToken["companyName"];
                    card.CompanyNameTextColor = (string)cardToken["companyNameTextColor"];
                    card.AccessToken = (string)cardToken["accessKey"];
                    card.Owner = user;

                    var after = user.Cards.Count();

                    var outlets = cardToken["outlets"] as JArray;
                    if (outlets != null)
                    {
                        foreach (var outlet in outlets)
                        {
                            var o = new Outlet();
                            o.Name = (string)outlet["name"];
                            o.Type = (string)outlet["type"];
                            o.Handle = (string)outlet["handle"];
                            o.Card = card;
                            card.Outlets.Add(o);
                        }
                    }
                    realm.Add(card, true);

                    transaction.Card = card;
                    transaction.Person = user;
                });

                return transaction;
            }
        }

        public static class EmailController
        {
            public static string Mapping = "email";

            public static async Task<NVCRestResult> SendValidationCode(string email)
            {
                NVCEndpoint endpoint = new NVCEndpoint();
                endpoint.Method = NVCEndpoint.EndpointType.PostRaw;
                endpoint.Controller = Mapping;

                Dictionary<string, object> requestDictionary = new Dictionary<string, object>();
                requestDictionary.Add("email", email);

                return await NVCRestHelper.Async(endpoint, requestDictionary, null);
            }


            public static async Task<NVCRestResult> UseValidationCode(string email, string code)
            {
                NVCEndpoint endpoint = new NVCEndpoint();
                endpoint.Method = NVCEndpoint.EndpointType.PostRaw;
                endpoint.Controller = Mapping;

                Dictionary<string, object> requestDictionary = new Dictionary<string, object>();
                requestDictionary.Add("email", email);
                requestDictionary.Add("code", code);

                return await NVCRestHelper.Async(endpoint, requestDictionary, null);
            }

        }
    }
}