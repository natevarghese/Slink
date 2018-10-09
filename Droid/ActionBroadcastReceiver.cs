using System;
using Android.Content;
using Slink.Droid.Services;

namespace Slink.Droid
{
    public class ActionBroadcastReceiver : BroadcastReceiver
    {
        private IOnDataRecievedListener mListener;
        public Action<Intent> NotificationReceived;

        public ActionBroadcastReceiver(IOnDataRecievedListener listener)
        {
            mListener = listener;
        }
        public ActionBroadcastReceiver()
        {

        }

        public override void OnReceive(Context context, Intent intent)
        {
            NotificationReceived?.Invoke(intent);

            var title = intent.GetStringExtra(Strings.InternalNotifications.notification_design_changed);
             if (String.IsNullOrEmpty(title)) return;
            var designChangedReciever = new DesignChangedReciever(FlyingObjectsFragment.onDataRecievedListener);

        }



        public interface IOnDataRecievedListener
        {
            void RecieveData();
        }
    }
}
