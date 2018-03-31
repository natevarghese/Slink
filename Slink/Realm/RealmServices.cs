using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Plugin.Geolocator.Abstractions;

namespace Slink
{
    public static class RealmServices
    {
        async public static Task<string> BoardcastCard(Card card, string UUID)
        {
            var permissionStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            if (permissionStatus != PermissionStatus.Granted) return null;

            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;

            Position position = null;
            try
            {
                position = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));
            }
            catch (Exception e)
            {
                AppCenterManager.Report(e);
            }
            if (position == null) return null;

            var lat = position.Latitude;
            var lon = position.Longitude;
            var time = DateTimeOffset.UtcNow;

            //get users name to replace the card name
            var me = RealmUserServices.GetMe(false);
            if (me == null) return null;

            try
            {
                await WebServices.UserController.UpdateUser(lat, lon);
                var transactionId = await WebServices.TransactionsController.CreateTransaction(lat, lon, card, me.Name);
                return transactionId;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            return null;
        }

        public static List<Outlet> GetAllAvailableOutlets()
        {
            var realm = RealmManager.SharedInstance.GetRealm(null);
            var returnList = new List<Outlet>();

            var facebook = new Outlet();
            facebook.Type = Outlet.outlet_type_facebook;
            facebook.AvailbleForAddition = realm.All<Outlet>().Where(o => o.Type.Equals(Outlet.outlet_type_facebook, StringComparison.OrdinalIgnoreCase)).Count() == 0;
            returnList.Add(facebook);

            var instagram = new Outlet();
            instagram.Type = Outlet.outlet_type_instagram;
            instagram.AvailbleForAddition = true;
            returnList.Add(instagram);

            var website = new Outlet();
            website.Type = Outlet.outlet_type_website;
            website.AvailbleForAddition = true;
            returnList.Add(website);

            //var email = new Outlet();
            //email.Type = Outlet.outlet_type_email;
            //email.AvailbleForAddition = true;
            //returnList.Add(email);

            var github = new Outlet();
            github.Type = Outlet.outlet_type_github;
            github.AvailbleForAddition = true;
            returnList.Add(github);

            var twitter = new Outlet();
            twitter.Type = Outlet.outlet_type_twitter;
            twitter.AvailbleForAddition = true;
            returnList.Add(twitter);

            var linkedIn = new Outlet();
            linkedIn.Type = Outlet.outlet_type_linkedIn;
            linkedIn.AvailbleForAddition = true;
            returnList.Add(linkedIn);

            var pinterest = new Outlet();
            pinterest.Type = Outlet.outlet_type_pinterest;
            pinterest.AvailbleForAddition = true;
            returnList.Add(pinterest);

            var google = new Outlet();
            google.Type = Outlet.outlet_type_google;
            google.AvailbleForAddition = true;
            returnList.Add(google);

            var phone = new Outlet();
            phone.Type = Outlet.outlet_type_phone;
            phone.AvailbleForAddition = true;
            returnList.Add(phone);

            return returnList.OrderBy(c => c.Type).ToList();
        }
        //public static AvailableOutlet GetAvailableOutlet(string type)
        //{
        //    var realm = RealmManager.SharedInstance.GetRealm(null);
        //    return realm.All<AvailableOutlet>().Where(c => c.Type.Equals(type, StringComparison.OrdinalIgnoreCase)).First();
        //}
        public static List<Card> GetMyCards(bool ForceToFront)
        {
            var realm = RealmManager.SharedInstance.GetRealm(null);

            var me = RealmUserServices.GetMe(false);
            if (me == null) return null;

            var list = realm.All<Card>().Where(c => c.Owner == me && c.Deleted == false).OrderBy(c => c.Name);
            if (ForceToFront)
            {
                realm.Write(() =>
                {
                    foreach (Card card in list)
                        card.IsFlipped = false;
                });
            }

            var returnList = list.ToList();
            return returnList;
        }

        public static IList<Outlet> GetMyOutlets()
        {
            var me = RealmUserServices.GetMe(false);
            if (me == null) return null;

            return me.Outlets.Where(c => c.Deleted == false).ToList();
        }
        public static List<SlinkUser> GetMyConnections()
        {
            var me = RealmUserServices.GetMe(false);

            var realm = RealmManager.SharedInstance.GetRealm(null);

            var owners = realm.All<SlinkUser>().Where(c => !c.ID.Equals(me.ID, StringComparison.OrdinalIgnoreCase)).OrderBy(c => c.FacebookID).ToList();

            var users = new List<SlinkUser>();
            foreach (var owner in owners)
            {
                //var targets = cards.Where(c => c.Owner == owner);
                foreach (Card c in owner.Cards)
                    c.ShowFront();

                //name adjustment required cuz shlomo put the name of the user inside the name of the card
                var firstCard = owner.Cards.FirstOrDefault();
                if (firstCard == null) continue;

                realm.Write(() =>
                {
                    owner.FirstName = firstCard.Name;
                });

                users.Add(owner);
            }
            return users;
        }
        public static bool SaveOutlet(Outlet outlet)
        {
            if (outlet == null) return false;
            if (String.IsNullOrEmpty(outlet.Handle)) return false;

            var realm = RealmManager.SharedInstance.GetRealm(null);
            if (realm == null) return false;

            bool duplicateFound = false;
            var allOutletsOfGivenHandle = realm.All<Outlet>().Where(o => o.Handle != null && o.Handle.Equals(outlet.Handle, StringComparison.OrdinalIgnoreCase)).ToList();

            if (allOutletsOfGivenHandle != null && allOutletsOfGivenHandle.Count() > 0)
            {
                foreach (Outlet o in allOutletsOfGivenHandle)
                {
                    if (o.Type.Equals(outlet.Type, StringComparison.OrdinalIgnoreCase))
                    {
                        duplicateFound = true;
                        break;
                    }
                }
            }



            if (!duplicateFound)
            {
                realm.Write(() =>
                {
                    realm.Add(outlet);

                    var me = RealmUserServices.GetMe(false);
                    me.Outlets.Add(outlet);
                });

                Transporter.SharedInstance.SetObject(Transporter.NewOutletAddedValueTransporterKey, outlet);
                return true;
            }
            return false;
        }

        public static void DeleteOutlet(Outlet outlet)
        {
            if (outlet == null) return;
            var realm = RealmManager.SharedInstance.GetRealm(null);

            var cards = RealmServices.GetMyCards(false);
            realm.Write(() =>
            {
                outlet.Deleted = true;

                //delete any card that has no outlets
                foreach (var card in cards)
                {
                    if (card.Outlets.Where(c => c.Deleted == false).Count() == 0)
                        card.Deleted = true;
                }
            });
        }
        public static bool DoesCardWithNameExist(string name)
        {
            if (String.IsNullOrEmpty(name)) return true;

            var target = name.Trim();
            var realm = RealmManager.SharedInstance.GetRealm(null);
            return realm.All<Card>().Where(c => c.Name.Equals(target, StringComparison.OrdinalIgnoreCase)).Count() > 0;
        }


        public static void SaveUserLocation(UserLocation userLocation)
        {
            var realm = RealmManager.SharedInstance.GetRealm(null);

            realm.Write(() =>
            {
                realm.RemoveAll<UserLocation>();
                realm.Add(userLocation);
            });
        }
        public static UserLocation GetLastUserLocation()
        {
            var realm = RealmManager.SharedInstance.GetRealm(null);
            return realm.All<UserLocation>().LastOrDefault();
        }
        public static void ShareAllOutlets(Card card)
        {
            var realm = RealmManager.SharedInstance.GetRealm(null);
            realm.Write(() =>
            {
                foreach (var outlet in card.Outlets)
                    outlet.Omitted = false;
            });
        }


        #region Discover
        public static void AcceptCard(Card card)
        {
            if (card == null || String.IsNullOrEmpty(card.UUID))
            {
                System.Diagnostics.Debug.WriteLine("Card null or empty uuid");
                return;
            }


            var realm = RealmManager.SharedInstance.GetRealm(null);

            var test = card.Owner.Cards.ToList();
            realm.Write(() =>
            {
                card.Retained = true;
            });
        }

        public static bool HaveIAlreadySeenThisCard(Card card)
        {
            if (card == null || String.IsNullOrEmpty(card.UUID)) return true;

            var realm = RealmManager.SharedInstance.GetRealm(null);
            return realm.All<Card>().Where(c => c.UUID.Equals(card.UUID, StringComparison.OrdinalIgnoreCase)).Count() > 0;
        }
        public static System.Linq.IQueryable<Card> GetUnretainedCards()
        {
            var realm = RealmManager.SharedInstance.GetRealm(null);
            var returnList = realm.All<Card>().Where(c => !c.Retained);

            System.Diagnostics.Debug.WriteLine("cards count here: " + returnList.Count());

            return returnList;
        }
        public static void DeleteAllUnretained()
        {
            var realm = RealmManager.SharedInstance.GetRealm(null);

            var unretained = realm.All<Card>().Where(c => !c.Retained);
            using (var trans = realm.BeginWrite())
            {
                foreach (var card in unretained)
                {
                    if (!card.IsMine())
                        realm.Remove(card);
                }
                trans.Commit();
            }
        }

        #endregion
    }
}
