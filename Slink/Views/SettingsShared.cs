using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using Plugin.DeviceInfo;
using Plugin.DeviceInfo.Abstractions;
using System.Diagnostics;
using Android.Widget;

namespace Slink
{
    public class SettingsShared
    {
        public static string navigation_item_my_outlets = "My Outlets";
        public static string navigation_item_edit_profile = "Edit Profile";
        public static string navigation_item_design = "Design";

        public static string navigation_item_logout = "Logout";

        public static string ItemClickedBroadcastReceiverKey = "ItemClicked";
        public static string ItemClickedBroadcastReceiverKeyPosition = "Position";
        public static string ItemClickedBroadcastReceiverKeyValue = "Value";

        public SettingsModel target;

        LinearLayout Parent;

        public List<SettingsModel> TableData;

        public List<SettingsModel> GetTableItems()
        {
           
            if (TableData != null && TableData.Count != 0) return TableData;

            TableData = new List<SettingsModel>();

            var myOutlets = new SettingsModel();
            myOutlets.Title = navigation_item_my_outlets;
            TableData.Add(myOutlets);

            var editProfile = new SettingsModel();
            editProfile.Title = navigation_item_edit_profile;
            TableData.Add(editProfile);

            var ipersistantStorageService = ServiceLocator.Instance.Resolve<IPersistantStorage>();
            var valueSaved = ipersistantStorageService?.GetDesignType();
            var val = valueSaved ?? Strings.DesignTypes.design_type_flying_colors;


            var design = new SettingsModel();
            design.Title = navigation_item_design;
            design.Value = val;
         

            if (CrossDeviceInfo.Current.Platform == Platform.iOS || CrossDeviceInfo.Current.Platform == Platform.Android)
                design.Values = new List<string>() { Strings.DesignTypes.design_type_flying_colors, Strings.DesignTypes.design_type_flying_lights, Strings.DesignTypes.design_type_none };
           
            TableData.Add(design);

            var logout = new SettingsModel();
            logout.Title = navigation_item_logout;
            TableData.Add(logout);

            return TableData;
        }

        public static void Logout(CancellationToken cancellationToken)
        {
            RealmUserServices.Logout();
        }

        public void DesignChanged()
        {
             target = GetTableItems().Where(c => c.Title.Equals(navigation_item_design, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            if (target == null) 
                return;
            if (target.Values == null || target.Values.Count == 0) return;
           //Debug.WriteLine(target.Values.Count);
            //if (CrossDeviceInfo.Current.Platform == Platform.Android)
            //{
              

               //    if (target.Values[i] == "FlyingColors")
               //    {

               //    }


               //        else if (target.Values[i] == "FlyingLights")
               //        {

               //        }

               //       else if (target.Values[i] == "None")
               //        {

               //        }

               //    }
               //}

            //on button click change the target button value
               var currentValIndex = target.Values.IndexOf(target.Value);
                currentValIndex = currentValIndex + 1 == target.Values.Count ? 0 : currentValIndex + 1;

                var newVal = target.Values[currentValIndex];
                target.Value = newVal;

           



                var ipersistantStorageService = ServiceLocator.Instance.Resolve<IPersistantStorage>();
                if (ipersistantStorageService == null) return;
                ipersistantStorageService.SetDesignType(newVal);

                var ibroadcastNotificaionService = ServiceLocator.Instance.Resolve<IBroadcastNotificaion>();
                if (ibroadcastNotificaionService == null) return;
                ibroadcastNotificaionService.SendNotificaion(Strings.InternalNotifications.notification_design_changed);

            

           
        }
        public void Trim()
        {
            TableData.Clear();
            TableData = null;
        }
        public class SettingsModel
        {
            public string Title { get; set; }
            public string Value { get; set; }
            public List<string> Values { get; set; }
        }
    }
}
