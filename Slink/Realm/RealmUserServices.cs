using System;
using System.Linq;
using System.Threading.Tasks;
using Realms;
using Realms.Sync;

namespace Slink
{
    public static class RealmUserServices
    {
        public static bool DidUserPersist()
        {
            try
            {
                var user = User.Current;  // if still logged in from last session
                if (user == null) return false;

                var iPersistantStorage = ServiceLocator.Instance.Resolve<IPersistantStorage>();
                if (iPersistantStorage == null) return false;

                var firstName = iPersistantStorage.GetFirstName();
                var lastName = iPersistantStorage.GetLastName();
                if (String.IsNullOrEmpty(firstName) || String.IsNullOrEmpty(lastName)) return false;

                var facebookToken = iPersistantStorage.GetFacebookToken();
                System.Diagnostics.Debug.WriteLine("FacebookToken From IPersistantStorage: " + facebookToken);
                if (String.IsNullOrEmpty(facebookToken)) return false;

                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
                return false;
            }
        }
        public static async Task LoginToServerAsync(string facebookToken)
        {
            System.Diagnostics.Debug.WriteLine("facebookToken:" + facebookToken);
            //AppCenterManager.Report("Facebook: " + facebookToken);

            var credentials = Credentials.Facebook(facebookToken);
            await User.LoginAsync(credentials, new Uri(NotSensitive.SystemUrls.realm_server_ip));

            if (User.Current == null)
            {
                AppCenterManager.Report("4.3");
                throw new Exception();
            }
        }
        async public static void Logout()
        {
            if (User.Current == null) return;

            await User.Current.LogOutAsync();
        }

        public static string GetMyRealmIdentity()
        {
            if (User.Current == null) return String.Empty;

            var userid = ServiceLocator.Instance.Resolve<IPersistantStorage>().GetUserId();
            if (String.IsNullOrEmpty(userid)) return String.Empty;

            return userid;
        }

        public static SlinkUser GetMe(bool shouldCreateNewUserIfNull)
        {
            if (User.Current == null) return null;

            var userid = ServiceLocator.Instance.Resolve<IPersistantStorage>().GetUserId();
            if (String.IsNullOrEmpty(userid)) return null;

            var realm = RealmManager.SharedInstance.GetRealm(null);
            var me = realm.All<SlinkUser>().Where(d => d.ID.Equals(userid, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            if (me == null && shouldCreateNewUserIfNull)
                me = SlinkUser.Create();

            return me;
        }
    }
}
