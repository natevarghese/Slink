using System;
using System.Collections.Generic;
using Realms;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Amazon.S3.Model;
using PCLStorage;
using System.Threading.Tasks;
using System.Linq;
using Realms.Sync;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Slink
{
    public class Card : RealmObject
    {
        [PrimaryKey]
        public string UUID { get; set; }
        public string Name { get; set; }
        public string AccessToken { get; set; }
        public bool IsFlipped { get; set; }
        public bool Deleted { get; set; }
        public bool Retained { get; set; }

        public SlinkUser Owner { get; set; }

        string BaseLocalURL
        {
            get
            {
                return FileSystem.Current.LocalStorage.Path + "/Users/" + RealmUserServices.GetMyRealmIdentity() + "/Cards/" + UUID;
            }
            set { }
        }
        string BaseRemoteURL
        {
            get
            {
                return "users/" + RealmUserServices.GetMyRealmIdentity() + "/cards/" + UUID;
            }
            set { }
        }


        //Front
        public string UserDisplayName { get; set; }
        public string Title { get; set; }
        public string BorderColor { get; set; }

        public IList<Outlet> Outlets { get;  }

        string remoteHeaderUrlCached;
        public string GetRemoteHeaderUrlCached()
        {
            if (String.IsNullOrEmpty(remoteHeaderUrlCached))
                remoteHeaderUrlCached = S3Utils.GetPresignedURL(RemoteHeaderURL, "Header.png", S3Utils.DefaultExpiry);
            return remoteHeaderUrlCached;
        }
        public string LocalHeaderURL
        {
            get
            {
                return BaseLocalURL + "/Header/Header.png";
            }
            set { }
        }
        public string RemoteHeaderURL
        {
            get
            {
                return BaseRemoteURL + "/header";
            }
            set { }
        }


        //Back
        public string BackgroundColor { get; set; }
        public string CompanyName { get; set; }
        public string CompanyNameTextColor { get; set; }

        string remoteLogoUrlCached;
        public string GetRemoteLogoUrlCached()
        {
            remoteLogoUrlCached = String.IsNullOrEmpty(remoteLogoUrlCached) ? S3Utils.GetPresignedURL(RemoteLogoURL, "Logo.png", S3Utils.DefaultExpiry) : remoteLogoUrlCached;
            return remoteLogoUrlCached;

        }
        public string LocalLogoURL
        {
            get
            {
                return BaseLocalURL + "/Logo/Logo.png";
            }
            set { }
        }
        public string RemoteLogoURL
        {
            get
            {
                return BaseRemoteURL + "/logo";
            }
            set { }
        }



        public void ShowFront()
        {
            var realm = RealmManager.SharedInstance.GetRealm(null);
            realm.Write(() =>
            {
                IsFlipped = false;
            });
        }
        public void ShowBack()
        {
            var realm = RealmManager.SharedInstance.GetRealm(null);
            realm.Write(() =>
            {
                IsFlipped = true;
            });
        }
        public void Flip()
        {
            var realm = RealmManager.SharedInstance.GetRealm(null);
            realm.Write(() =>
            {
                IsFlipped = !IsFlipped;
            });
        }
        public bool IsMine()
        {
            if (Owner == null) return false;

            var userid = ServiceLocator.Instance.Resolve<IPersistantStorage>().GetUserId();
            if (String.IsNullOrEmpty(userid)) return false;

            var result = Owner.ID == null || Owner.ID.Equals(userid, StringComparison.OrdinalIgnoreCase);
            return result;
        }
        public void AddOutlet(Outlet outlet)
        {
            var realm = RealmManager.SharedInstance.GetRealm(null);
            var item = Outlets.Where(o => o.Type.Equals(outlet.Type, StringComparison.OrdinalIgnoreCase) && o.Handle.Equals(outlet.Handle, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (item == null) //prevent adding the same outlet twice.
            {
                realm.Write(() =>
                {
                    Outlets.Add(outlet);
                });
            }
        }
        public void RemoveOutlet(Outlet outlet)
        {
            var realm = RealmManager.SharedInstance.GetRealm(null);
            realm.Write(() =>
            {
                Outlets.Remove(outlet);
            });
        }
        public void UpdateStringProperty(Expression<Func<string>> property, string newValue)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
            }
            else
            {
                var realm = RealmManager.SharedInstance.GetRealm(null);
                realm.Write(() =>
                {
                    propertyInfo.SetValue(this, newValue);
                });
            }
        }

        public JObject ToJObject()
        {
            var returnObj = new JObject();
            returnObj["name"] = Name;
            returnObj["userDisplayName"] = UserDisplayName;
            returnObj["title"] = Title;
            returnObj["uuid"] = UUID;
            returnObj["borderColor"] = BorderColor;
            returnObj["backgroundColor"] = BackgroundColor;
            returnObj["companyName"] = CompanyName;
            returnObj["companyNameTextColor"] = CompanyNameTextColor;

            JArray outletsArray = new JArray();
            foreach (var outlet in Outlets.Where(c => !c.Omitted))
            {
                var jobjectOutlet = outlet.ToJObject();
                outletsArray.Add(jobjectOutlet);
            }
            returnObj["outlets"] = outletsArray;
            return returnObj;
        }

        public static Card Create(JToken cardToken)
        {
            //todo finish
            var card = new Card();
            if (cardToken != null)
            {
                card.Name = (string)cardToken["name"];
                card.UserDisplayName = (string)cardToken["userDisplayName"];
                card.Title = (string)cardToken["title"];
                card.UUID = (string)cardToken["uuid"];
                card.BorderColor = (string)cardToken["borderColor"];
                card.BackgroundColor = (string)cardToken["backgroundColor"];
                card.CompanyName = (string)cardToken["companyName"];
                card.CompanyNameTextColor = (string)cardToken["companyNameTextColor"];

                var outlets = (JArray)cardToken["outlets"];
                foreach (var outletToken in outlets)
                {
                    Outlet outlet = new Outlet();
                    outlet.Name = (string)outletToken["name"];
                    outlet.Type = (string)outletToken["type"];
                    outlet.Handle = (string)outletToken["handle"];
                    card.Outlets.Add(outlet);
                }
            }
            return card;
        }


        public static Card Create()
        {
            var me = RealmUserServices.GetMe(false);
            if (me == null) return null;

            var card = new Card();
            card.CompanyNameTextColor = ColorUtils.ToHexString(ColorUtils.GetColor(ColorUtils.ColorType.Black));
            card.BorderColor = ColorUtils.ToHexString(ColorUtils.GetColor(ColorUtils.ColorType.White));
            card.BackgroundColor = ColorUtils.ToHexString(ColorUtils.GetColor(ColorUtils.ColorType.White));
            card.Name = Strings.Basic.new_card;
            card.UUID = Guid.NewGuid().ToString();
            card.UserDisplayName = me.Name;
            card.Retained = true;

            var realm = RealmManager.SharedInstance.GetRealm(null);
            realm.Write(() =>
            {
                realm.Add(card);
            });

            return card;
        }
    }
}