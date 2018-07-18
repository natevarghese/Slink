using System;
using System.Linq;
using Android.Content;
using Android.Views;
using System.Timers;
using System.Threading.Tasks;

namespace Slink.Droid
{
    public class SharingRecyclerViewFragment : RecyclerViewFragment<SharingShared.Model>
    {
        public SharingShared Shared = new SharingShared();

        Timer Timer = new Timer();
        bool ButtonLocked;
        ActionBroadcastReceiver TapToShareBroadCastReceiver;

        public override View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            RecyclerViewAdapter.SetListItems(Shared.GetTableItemsAndroid());

            //Activity.Title = "";

            HasOptionsMenu = true;

            return view;
        }
        public override void OnResume()
        {
            base.OnResume();


            TapToShareBroadCastReceiver = new ActionBroadcastReceiver();
            TapToShareBroadCastReceiver.NotificationReceived += (obj) =>
            {
                Activity.RunOnUiThread(() =>
                {
                    if (Shared.State == SharingShared.SharingState.DisplayPurposesOnly)
                    {
                        //todo
                        return;
                    }

                    if (Shared.State == SharingShared.SharingState.PermissionDenied)
                    {
                        //todo
                        return;
                    }

                    //make sure at least one outlet is selected
                    if (!Shared.SelectedCard.Outlets.Any(c => !c.Omitted)) return;

                    if (ButtonLocked) return;
                    ButtonLocked = true;

                    if (Shared.Sharing)
                        StopSharing();
                    else
                        StartSharing();
                });
            };
            Activity.RegisterReceiver(TapToShareBroadCastReceiver, new IntentFilter(SharingShared.TapToShareBroadCastReceiverClicked));


        }


        public override void OnStop()
        {
            base.OnStop();

            if (TapToShareBroadCastReceiver != null)
                Activity.UnregisterReceiver(TapToShareBroadCastReceiver);


        }

        public override BaseRecyclerViewAdapter<SharingShared.Model> GetRecyclerViewAdapter()
        {
            var adapter = new SharingRecyclerViewAdapter(Activity);
            return adapter;
        }
        public override void RecyclerView_ItemClick(SharingShared.Model obj, int position)
        {
            base.RecyclerView_ItemClick(obj, position);

        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            var resource = Resource.Menu.toolbar_edit;
            inflater.Inflate(resource, menu);
            base.OnCreateOptionsMenu(menu, inflater);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.Save:
                    Transporter.SharedInstance.RemoveObject(Transporter.NewOutletAddedTransporterKey);
                    Transporter.SharedInstance.RemoveObject(Transporter.NewOutletAddedValueTransporterKey);

                    //var vc = new NewCardViewController();
                    //(vc.TargetViewController as NewCardTableViewController).Shared.SelectedCard = (TargetViewController as SharingTableViewController).Shared.SelectedCard;
                    //ApplicationExtensions.PushViewController(vc, true);

                    break;
                case Resource.Id.Delete:

                    //((SharingTableViewController)TargetViewController).Shared.DeleteCard();

                    var convertedActiviy = (Activity as BaseActivity);
                    convertedActiviy.HideKeyboard();
                    convertedActiviy.PopFragmentOver();

                    break;
                case Resource.Id.Edit:

                    break;
            }
            return base.OnOptionsItemSelected(item);
        }



        async public void StartSharing()
        {
            if (!Shared.CanStartSharing()) return;

            Shared.State = SharingShared.SharingState.Authenticating;
            RecyclerViewAdapter.SetListItems(Shared.GetTableItemsAndroid());

            var sharing = await Shared.ShareChard();
            if (sharing)
            {
                //Timer.Start();
                //ApplyAnimation();
            }
            else
            {
                Shared.State = SharingShared.SharingState.Failed;
                RecyclerViewAdapter.SetListItems(Shared.GetTableItemsAndroid());
            }

            ButtonLocked = false;
        }
        public void StopSharing()
        {
            Shared.Sharing = false;

            //if (shape != null)
            //{
            //    shape.RemoveAllAnimations();
            //    shape.RemoveFromSuperLayer();
            //}


            if (Shared.State == SharingShared.SharingState.DisplayPurposesOnly)
            {
                Shared.State = SharingShared.SharingState.DisplayPurposesOnly;
                RecyclerViewAdapter.SetListItems(Shared.GetTableItemsAndroid());
            }
            else if (Shared.State == SharingShared.SharingState.PermissionDenied)
            {
                Shared.State = SharingShared.SharingState.PermissionDenied;
                RecyclerViewAdapter.SetListItems(Shared.GetTableItemsAndroid());
            }
            else
            {
                Shared.State = SharingShared.SharingState.NotSharing;
                RecyclerViewAdapter.SetListItems(Shared.GetTableItemsAndroid());

                if (String.IsNullOrEmpty(Shared.SessionUUID)) return;

                Task.Run(async () =>
                {
                    if (String.IsNullOrEmpty(Shared.SessionUUID)) return;
                    await WebServices.TransactionsController.TerminateTransaction(Shared.SessionUUID);
                });
            }

            ButtonLocked = false;

            Timer.Stop();
        }
    }
}
