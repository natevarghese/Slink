using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Geolocator;

namespace Slink
{
    public class DiscoverShared
    {

        public List<DiscoverModel> TableItems = new List<DiscoverModel>();
        public List<Card> AllItems = new List<Card>();

        public DiscoverShared() { RealmServices.DeleteAllUnretained(); }

        public static bool QueryLock = false;
        async public Task GetNearbyTransactions()
        {
            if (QueryLock) return;
            QueryLock = true;

            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;

                var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));
                if (position == null) return;

                var lat = position.Latitude;
                var lon = position.Longitude;

                var me = RealmUserServices.GetMe(false);
                if (me == null) return;

                var id = ServiceLocator.Instance.Resolve<IPersistantStorage>().GetUserId();
                if (String.IsNullOrEmpty(id)) return;

                await WebServices.UserController.UpdateUser(lat, lon);
                await WebServices.TransactionsController.GetTranactions(AllItems);
                var cards = RealmServices.GetUnretainedCards();
                var filteredCards = FilterCards(cards);

                System.Diagnostics.Debug.WriteLine("cards count: " + filteredCards.Count());

                foreach (var card in filteredCards)
                {
                    //don't show my own broadcasts
                    var isMine = card.Owner.FacebookID.Equals(id, StringComparison.OrdinalIgnoreCase);
                    if (isMine) continue;

                    var model = new DiscoverModel();
                    model.Card = card;
                    TableItems.Add(model);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            QueryLock = false;
        }
        List<Card> FilterCards(System.Linq.IQueryable<Card> cards)
        {
            var returnList = new List<Card>();
            foreach (var card in cards)
            {
                var target = AllItems.Where(c => c.UUID.Equals(card.UUID, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (target == null)
                    returnList.Add(card);
            }
            return returnList;
        }
        public DiscoverModel GetNextCard()
        {
            var firstInList = TableItems.Where(c => !c.Showing).FirstOrDefault();
            if (firstInList == null) return null;
            firstInList.Showing = true;

            return firstInList;
        }
        public void AcceptCard()
        {
            var discoverModel = TableItems.FirstOrDefault();
            if (discoverModel == null) return;

            RealmServices.AcceptCard(discoverModel.Card);
            TableItems.Remove(discoverModel);
            AllItems.Add(discoverModel.Card);
        }
        public void RejectCard()
        {
            var discoverModel = TableItems.FirstOrDefault();
            if (discoverModel == null) return;

            TableItems.Remove(discoverModel);
            AllItems.Add(discoverModel.Card);
        }
        public bool ShouldStopSearching()
        {
            bool anyNotDecided = TableItems.Any(c => !c.Card.Retained);

            return anyNotDecided;
        }
        public class DiscoverModel
        {
            public bool Showing { get; set; }
            public Card Card { get; set; }
        }
    }
}
