using System.Collections.Generic;
using Realms;
using Realms.Sync;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PCLStorage;

namespace Slink
{
    public class SlinkUser : RealmObject
    {
        [PrimaryKey]
        public string ID { get; set; }
        public string FacebookID { get; set; }
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Handle { get; set; }

        string BaseLocalURL
        {
            get
            {
                return FileSystem.Current.LocalStorage.Path + "/Users/" + RealmUserServices.GetMyRealmIdentity() + "/" + "Profile";
            }
            set { }
        }
        string BaseRemoteURL
        {
            get
            {
                return "users/" + RealmUserServices.GetMyRealmIdentity();
            }
            set { }
        }

        string remoteProfileImageUrlCached;
        public string GetRemoteProfileImageUrlCached()
        {
            remoteProfileImageUrlCached = String.IsNullOrEmpty(remoteProfileImageUrlCached) ? S3Utils.GetPresignedURL(RemoteProfileImageURL, "Profile.png", S3Utils.DefaultExpiry) : remoteProfileImageUrlCached;
            return remoteProfileImageUrlCached;
        }
        public string LocalProfileImageURL
        {
            get
            {
                return BaseLocalURL + "/Profile.png";
            }
            set { }
        }
        public string RemoteProfileImageURL
        {
            get
            {
                return BaseRemoteURL + "/profile";
            }
            set { }
        }

        public string Name
        {
            get
            {
                return (FirstName?.Trim() + " " + LastName?.Trim()).Trim();
            }
            set { }
        }


        [Backlink(nameof(Card.Owner))]
        public IQueryable<Card> Cards { get; }

        public IList<Outlet> Outlets { get; }

        public static SlinkUser Create()
        {
            var returnUser = new SlinkUser();

            var userid = ServiceLocator.Instance.Resolve<IPersistantStorage>().GetUserId();
            returnUser.ID = userid;

            var realm = RealmManager.SharedInstance.GetRealm(null);
            realm.Write(() =>
            {
                realm.Add(returnUser);
            });
            return returnUser;
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
        public string GetFacebookProfilePictureUrl()
        {
            var persistantStorage = ServiceLocator.Instance.Resolve<IPersistantStorage>();
            if (persistantStorage == null) return null;

            var userid = persistantStorage.GetUserId();
            if (String.IsNullOrEmpty(userid)) return null;

            return "https://graph.facebook.com/" + userid + "/picture?type=large";
        }
        public static void SetNextHandelByNameIfNecessary()
        {
            var me = RealmUserServices.GetMe(false);
            if (me == null) return;
            if (!String.IsNullOrEmpty(me.Handle)) return;

            if (String.IsNullOrEmpty(me.FirstName) || String.IsNullOrEmpty(me.LastName)) throw new Exception("User Information Incomplete");

            string finalHandel = null;
            int suffix = 0;
            string potentialHandle = "@" + me.FirstName + "_" + me.LastName;
            var realm = RealmManager.SharedInstance.GetRealm(null);

            while (String.IsNullOrEmpty(finalHandel))
            {
                var userWithHandleFound = realm.All<SlinkUser>().Where(u => u.Handle.Equals(potentialHandle, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (userWithHandleFound == null) //no user was found
                {
                    finalHandel = potentialHandle;
                    break;
                }

                suffix++;
                potentialHandle = "@" + me.FirstName + "_" + me.LastName + suffix;
            }


            realm.Write(() =>
            {
                me.Handle = finalHandel;
            });
        }
    }
}
