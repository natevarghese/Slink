using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Slink
{
    public class LoginShared
    {
        public List<string> FacebookPermissions = new List<string> { "public_profile", "email", "user_friends" };
        public string FacebookFirstName, FacebookLastName, FacebookFullName, FacebookUserId;

        public void SetUserData(string userId, string firstName, string lastName)
        {
            IPersistantStorage iPersistant = ServiceLocator.Instance.Resolve<IPersistantStorage>();
            if (iPersistant == null) return;

            iPersistant.SetUserId(userId);
            iPersistant.SetFirstName(firstName);
            iPersistant.SetLastName(lastName);
        }

        async public Task<bool> CreateUser(string facebookToken)
        {
            IPersistantStorage iPersistant = ServiceLocator.Instance.Resolve<IPersistantStorage>();
            if (iPersistant == null) return false;
            iPersistant.SetFacebookToken(facebookToken);

            try
            {
                await RealmUserServices.LoginToServerAsync(facebookToken);
            }
            catch (Exception er)
            {
                AppCenterManager.Report(er);
                return false;
            }

            try
            {
                var result = await WebServices.UserController.CreateUser();
                if (result.StatusCode == System.Net.HttpStatusCode.OK) return true;

                //user already exists, that's fine.
                if (result.StatusCode == System.Net.HttpStatusCode.Conflict) return true;
            }
            catch (Exception er)
            {
                AppCenterManager.Report(er);
            }

            return false;
        }


        public void NextPage()
        {
            var realm = RealmManager.SharedInstance.GetRealm(null);
            var me = RealmUserServices.GetMe(true);

            if (!String.IsNullOrEmpty(FacebookFirstName) || !String.IsNullOrEmpty(FacebookLastName))
            {
                realm.Write(() =>
                {
                    if (!String.IsNullOrEmpty(FacebookUserId))
                        me.FacebookID = FacebookUserId;

                    if (!String.IsNullOrEmpty(FacebookFirstName))
                        me.FirstName = FacebookFirstName;

                    if (!String.IsNullOrEmpty(FacebookLastName))
                        me.LastName = FacebookLastName;
                });
            }

            var outlet = new Outlet();
            outlet.Name = FacebookFullName;
            outlet.Handle = FacebookUserId;
            outlet.Locked = true;
            outlet.Type = Outlet.outlet_type_facebook;
            RealmServices.SaveOutlet(outlet);
        }
    }
}
