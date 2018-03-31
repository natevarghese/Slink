using System;
using System.Globalization;
using Foundation;
using UIKit;

namespace Slink.iOS
{
    public class PersistantStorage : IPersistantStorage
    {

        public string GetUserId()
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(Strings.PersistantKeys.persistant_key_user_id);
        }
        public string GetEmail()
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(Strings.PersistantKeys.persistant_key_email);
        }

        public void RemoveAll()
        {
            NSUserDefaults defaults = NSUserDefaults.StandardUserDefaults;
            defaults.RemovePersistentDomain(NSBundle.MainBundle.BundleIdentifier);
            defaults.Synchronize();
        }

        public void SetUserId(string userid)
        {
            SetToDefaults(Strings.PersistantKeys.persistant_key_user_id, userid);
        }
        public void SetEmail(string email)
        {
            SetToDefaults(Strings.PersistantKeys.persistant_key_email, email);
        }

        public void SetToDefaults(string key, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                NSUserDefaults defaults = NSUserDefaults.StandardUserDefaults;
                defaults.SetString(value, key);
                defaults.Synchronize();
            }
        }

        public void SetToDefaults(string key, int value)
        {
            NSUserDefaults defaults = NSUserDefaults.StandardUserDefaults;
            defaults.SetInt(value, key);
            defaults.Synchronize();
        }

        public string GetPassword()
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(Strings.PersistantKeys.persistant_key_user_password);
        }

        public void SetPassword(string password)
        {
            SetToDefaults(Strings.PersistantKeys.persistant_key_user_password, password);
        }

        public string GetAccessToken()
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(Strings.PersistantKeys.persistant_key_access_token);
        }

        public void SetAccessToken(string token)
        {
            SetToDefaults(Strings.PersistantKeys.persistant_key_access_token, token);
        }

        public string GetRefreshToken()
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(Strings.PersistantKeys.persistant_key_refresh_token);
        }

        public void SetRefreshToken(string token)
        {
            SetToDefaults(Strings.PersistantKeys.persistant_key_refresh_token, token);
        }

        public DateTime GetTokenExpiry()
        {
            string dateTimeString = NSUserDefaults.StandardUserDefaults.StringForKey(Strings.PersistantKeys.persistant_key_access_expiry);
            if (String.IsNullOrEmpty(dateTimeString)) return DateTime.Now.AddSeconds(-1);

            return DateTime.ParseExact(dateTimeString, "O", CultureInfo.InvariantCulture);
        }

        public void SetTokenExpiry(DateTime expiry)
        {
            SetToDefaults(Strings.PersistantKeys.persistant_key_access_expiry, expiry.ToString("O"));
        }

        public string GetAccessScope()
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(Strings.PersistantKeys.persistant_key_access_scope);
        }

        public void SetAccessScope(string scope)
        {
            SetToDefaults(Strings.PersistantKeys.persistant_key_access_scope, scope);
        }

        public string GetFacebookToken()
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(Strings.PersistantKeys.persistant_key_facebook_access_token);
        }

        public void SetFacebookToken(string scope)
        {
            SetToDefaults(Strings.PersistantKeys.persistant_key_facebook_access_token, scope);
        }

        public DateTime GetFacebookTokenExpiry()
        {
            string dateTimeString = NSUserDefaults.StandardUserDefaults.StringForKey(Strings.PersistantKeys.persistant_key_facebook_access_expiry);
            if (String.IsNullOrEmpty(dateTimeString)) return DateTime.Now.AddSeconds(-1);

            return DateTime.ParseExact(dateTimeString, "O", CultureInfo.InvariantCulture);
        }

        public void SetFacebookTokenExpiry(DateTime expiry)
        {
            SetToDefaults(Strings.PersistantKeys.persistant_key_facebook_access_expiry, expiry.ToString("O"));
        }

        public string GetFirstName()
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(Strings.PersistantKeys.persistant_key_first_name);
        }

        public void SetFirstName(string firstName)
        {
            SetToDefaults(Strings.PersistantKeys.persistant_key_first_name, firstName);
        }

        public string GetLastName()
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(Strings.PersistantKeys.persistant_key_last_name);
        }

        public void SetLastName(string lastName)
        {
            SetToDefaults(Strings.PersistantKeys.persistant_key_last_name, lastName);
        }

        public string GetPushToken()
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(Strings.PersistantKeys.persistant_key_push_token);
        }

        public void SetPushToken(string token)
        {
            SetToDefaults(Strings.PersistantKeys.persistant_key_push_token, token);
        }

        public int GetDiscoverNotificationCount()
        {
            return (int)NSUserDefaults.StandardUserDefaults.IntForKey(Strings.PersistantKeys.persistant_key_discover_count);
        }

        public void SetDiscoverNotificaionCount(int count)
        {
            SetToDefaults(Strings.PersistantKeys.persistant_key_discover_count, count);
        }
        public void IncrementDiscoverNotificaionCount(int incrementalValue)
        {
            var currentCount = GetDiscoverNotificationCount();
            currentCount = currentCount + incrementalValue;
            SetDiscoverNotificaionCount(currentCount);
        }

        public string GetDesignType()
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(Strings.PersistantKeys.persistant_key_design_type);
        }

        public void SetDesignType(string designType)
        {
            SetToDefaults(Strings.PersistantKeys.persistant_key_design_type, designType);
        }
    }
}
