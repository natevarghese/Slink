using System;
using System.Collections.Generic;
using System.Linq;

namespace Slink
{
    public class SharingShared
    {
        public Card SelectedCard;


        public IList<Outlet> GetTableItems()
        {
            var realm = RealmManager.SharedInstance.GetRealm(null);
            realm.Write(() =>
            {
                foreach (var outlet in SelectedCard.Outlets)
                    outlet.Omitted = false;
            });

            return SelectedCard.Outlets.OrderBy(c => c.Type).ToList();
        }

        public void OutletSelected(Outlet outlet)
        {
            var realm = RealmManager.SharedInstance.GetRealm(null);
            realm.Write(() =>
            {
                outlet.Omitted = !outlet.Omitted;
            });
        }
        public void DeleteCard()
        {
            var realm = RealmManager.SharedInstance.GetRealm(null);
            realm.Write(() =>
            {
                SelectedCard.Deleted = true;
            });
        }
    }
}
