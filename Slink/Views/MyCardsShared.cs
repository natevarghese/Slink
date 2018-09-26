using System;
using System.Collections.Generic;
using Realms;
using Plugin.DeviceInfo;
using Plugin.DeviceInfo.Abstractions;
using System.Linq;

namespace Slink
{
    public class MyCardsShared
    {
        public static string CreateNewCard = "Create New Card +";
        public static string ItemClickedBroadcastReceiverKeyCardClicked = "CardClicked";

        public static string ItemClickedBroadcastReceiverKeyPosition = "CellPosition";

        public List<Card> GetMyCards(bool forceToFront)
        {
            var cards = RealmServices.GetMyCards(forceToFront);

            //if (CrossDeviceInfo.Current.Platform == Platform.Android)
            //{
            cards.Add(null);
            //}

            return cards;
        }
    }
}