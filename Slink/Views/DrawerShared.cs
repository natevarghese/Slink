using System;
using System.Collections.Generic;
namespace Slink
{
    public class DrawerShared
    {

        public static string navigation_item_my_cards = "My Cards";
        public static string navigation_item_connections = "Connections";
        public static string navigation_item_settings = "Settings";
        public static string navigation_item_discover = "Discover";

        public static string GetFooterText()
        {
            return "© NVComputers " + DateTime.Now.Year.ToString() + ".  All Rights Reserved.";
        }

        public List<DrawerModel> GetTableItems()
        {
            var persistantStorageService = ServiceLocator.Instance.Resolve<IPersistantStorage>();
            if (persistantStorageService == null) return null;

            var returnList = new List<DrawerModel>();

            var myCards = new DrawerModel();
            myCards.Title = navigation_item_my_cards;
            myCards.Image = "Home";
            returnList.Add(myCards);


            var discover = new DrawerModel();
            discover.Title = navigation_item_discover;
            discover.Image = "Discover";
            discover.Notifications = persistantStorageService.GetDiscoverNotificationCount();
            returnList.Add(discover);

            var connections = new DrawerModel();
            connections.Title = navigation_item_connections;
            connections.Image = "Connections";
            returnList.Add(connections);

            var settings = new DrawerModel();
            settings.Title = navigation_item_settings;
            settings.Image = "Settings";
            returnList.Add(settings);


            return returnList;
        }


        public class DrawerModel
        {
            public string Title { get; set; }
            public string Image { get; set; }
            public int Notifications { get; set; }
        }
    }
}
