using System;
using System.Globalization;
using Android.Preferences;
using Plugin.CurrentActivity;

namespace Slink.Droid
{
    public class PersistantStorage : IPersistantStorage
    {

        public string GetUserId()
        {
            return GetFromDefaults(Strings.PersistantKeys.persistant_key_user_id, null);
        }
        public string GetEmail()
        {
            return GetFromDefaults(Strings.PersistantKeys.persistant_key_email, null);
        }

        public void RemoveAll()
        {
            var preferences = PreferenceManager.GetDefaultSharedPreferences(CrossCurrentActivity.Current.Activity);
            preferences.Edit().Clear().Commit();
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
            if (String.IsNullOrEmpty(value)) return;

            var preferences = PreferenceManager.GetDefaultSharedPreferences(CrossCurrentActivity.Current.Activity);
            preferences.Edit().PutString(key, value).Commit();
        }

        public void SetToDefaults(string key, int value)
        {
            var preferences = PreferenceManager.GetDefaultSharedPreferences(CrossCurrentActivity.Current.Activity);
            preferences.Edit().PutInt(key, value).Commit();
        }
        public string GetFromDefaults(string key, string defaultValue)
        {
            var preferences = PreferenceManager.GetDefaultSharedPreferences(CrossCurrentActivity.Current.Activity);
            return preferences.GetString(key, defaultValue);
        }

        public string GetPassword()
        {
            var preferences = PreferenceManager.GetDefaultSharedPreferences(CrossCurrentActivity.Current.Activity);
            return preferences.GetString(Strings.PersistantKeys.persistant_key_user_password, null);
        }

        public void SetPassword(string password)
        {
            SetToDefaults(Strings.PersistantKeys.persistant_key_user_password, password);
        }

        public string GetAccessToken()
        {
            return GetFromDefaults(Strings.PersistantKeys.persistant_key_access_token, null);
        }

        public void SetAccessToken(string token)
        {
            SetToDefaults(Strings.PersistantKeys.persistant_key_access_token, token);
        }

        public string GetRefreshToken()
        {
            return GetFromDefaults(Strings.PersistantKeys.persistant_key_refresh_token, null);
        }

        public void SetRefreshToken(string token)
        {
            SetToDefaults(Strings.PersistantKeys.persistant_key_refresh_token, token);
        }

        public DateTime GetTokenExpiry()
        {
            string dateTimeString = GetFromDefaults(Strings.PersistantKeys.persistant_key_access_expiry, null);
            if (String.IsNullOrEmpty(dateTimeString)) return DateTime.Now.AddSeconds(-1);

            return DateTime.ParseExact(dateTimeString, "O", CultureInfo.InvariantCulture);
        }

        public void SetTokenExpiry(DateTime expiry)
        {
            SetToDefaults(Strings.PersistantKeys.persistant_key_access_expiry, expiry.ToString("O"));
        }

        public string GetAccessScope()
        {
            return GetFromDefaults(Strings.PersistantKeys.persistant_key_access_scope, null);
        }

        public void SetAccessScope(string scope)
        {
            SetToDefaults(Strings.PersistantKeys.persistant_key_access_scope, scope);
        }

        public string GetFacebookToken()
        {
            return GetFromDefaults(Strings.PersistantKeys.persistant_key_facebook_access_token, null);
        }

        public void SetFacebookToken(string scope)
        {
            SetToDefaults(Strings.PersistantKeys.persistant_key_facebook_access_token, scope);
        }

        public DateTime GetFacebookTokenExpiry()
        {
            string dateTimeString = GetFromDefaults(Strings.PersistantKeys.persistant_key_facebook_access_expiry, null);
            if (String.IsNullOrEmpty(dateTimeString)) return DateTime.Now.AddSeconds(-1);

            return DateTime.ParseExact(dateTimeString, "O", CultureInfo.InvariantCulture);
        }

        public void SetFacebookTokenExpiry(DateTime expiry)
        {
            SetToDefaults(Strings.PersistantKeys.persistant_key_facebook_access_expiry, expiry.ToString("O"));
        }

        public string GetFirstName()
        {
            return GetFromDefaults(Strings.PersistantKeys.persistant_key_first_name, null);
        }

        public void SetFirstName(string firstName)
        {
            SetToDefaults(Strings.PersistantKeys.persistant_key_first_name, firstName);
        }

        public string GetLastName()
        {
            return GetFromDefaults(Strings.PersistantKeys.persistant_key_last_name, null);
        }

        public void SetLastName(string lastName)
        {
            SetToDefaults(Strings.PersistantKeys.persistant_key_last_name, lastName);
        }

        public string GetPushToken()
        {
            return GetFromDefaults(Strings.PersistantKeys.persistant_key_push_token, null);
        }

        public void SetPushToken(string token)
        {
            SetToDefaults(Strings.PersistantKeys.persistant_key_push_token, token);
        }

        public int GetDiscoverNotificationCount()
        {
            var preferences = PreferenceManager.GetDefaultSharedPreferences(CrossCurrentActivity.Current.Activity);
            return preferences.GetInt(Strings.PersistantKeys.persistant_key_discover_count, 0);
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
            return GetFromDefaults(Strings.PersistantKeys.persistant_key_design_type, null);
        }

        public void SetDesignType(string designType)
        {
            SetToDefaults(Strings.PersistantKeys.persistant_key_design_type, designType);
        }
    }
}
