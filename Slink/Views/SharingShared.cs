using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.DeviceInfo;
using Plugin.DeviceInfo.Abstractions;

namespace Slink
{
    public class SharingShared
    {
        public static string ItemClickedBroadcastReceiverKeyPosition = "CellPosition";
        public static string ItemClickedBroadcastReceiverKeyCardClicked = "CardClicked";
        public static string TapToShareBroadCastReceiverClicked = "TapToShareClicked";

        public string SessionUUID;
        public SharingState State = SharingState.NotSharing;
        public bool Sharing, DisplayPurposeOnly;
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
            footer.Object = GetFooterText();
            list.Add(footer);

            return list;
        }

        public bool CanStartSharing()
        {
            if (DisplayPurposeOnly) return false;
            if (SelectedCard == null) return false;
            if (!IsSufficentPermissionGranted()) return false;

            return true;
        }
        async public Task<bool> ShareChard()
        {
            try
            {
                SessionUUID = await RealmServices.BoardcastCard(SelectedCard, SessionUUID);
                Sharing = !String.IsNullOrEmpty(SessionUUID);
            }
            catch (Exception e)
            {
                Sharing = false;
                AppCenterManager.Report(e);
            }

            return Sharing;
        }



        public bool IsSufficentPermissionGranted()
        {
            return true;
        }
        string GetFooterText()
        {
            switch (State)
            {
                case SharingState.NotSharing:
                    return Strings.Sharing.tap_to_share;
                case SharingState.Sharing:
                    return Strings.Sharing.sharing;
                case SharingState.Failed:
                    return Strings.Sharing.could_not_share_card;
                case SharingState.DisplayPurposesOnly:
                    return String.Empty;
                case SharingState.Authenticating:
                    return Strings.Sharing.authenticating;
                case SharingState.PermissionDenied:
                    return Strings.Sharing.location_permission_necessary;
            }

            return null;
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


        public enum SharingState
        {
            NotSharing = 0,
            Sharing = 1,
            Failed = 2,
            DisplayPurposesOnly = 3,
            Authenticating = 4,
            PermissionDenied = 5,
        }
    }
}
