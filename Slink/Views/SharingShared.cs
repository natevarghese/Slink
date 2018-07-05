using System;
using System.Collections.Generic;
using System.Linq;
using Plugin.DeviceInfo;
using Plugin.DeviceInfo.Abstractions;

namespace Slink
{
    public class SharingShared
    {
        public static string ItemClickedBroadcastReceiverKeyPosition = "CellPosition";
        public static string ItemClickedBroadcastReceiverKeyCardClicked = "CardClicked";
        public static string TapToShareBroadCastReceiverClicked = "TapToShareClicked";

        public Card SelectedCard;

        public IList<Outlet> GetTableItems()
        {
            var realm = RealmManager.SharedInstance.GetRealm(null);
            realm.Write(() =>
            {
                foreach (var outlet in SelectedCard.Outlets)
                    outlet.Omitted = false;
            });

            var outlets = SelectedCard.Outlets.OrderBy(c => c.Type).ToList();
            return outlets;
        }

        //todo make sure to use this one for iOS too
        public List<Model> GetTableItemsAndroid()
        {
            var list = new List<Model>();

            var header = new Model();
            header.IsHeader = true;
            header.Object = SelectedCard;
            list.Add(header);

            foreach (var outlet in GetTableItems())
            {
                var obj = new Model();
                obj.Object = outlet;
                list.Add(obj);
            }

            var footer = new Model();
            footer.IsFooter = true;
            list.Add(footer);

            return list;
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


        public class Model
        {
            public bool IsHeader { get; set; }
            public bool IsFooter { get; set; }
            public object Object { get; set; }
        }
    }
}
