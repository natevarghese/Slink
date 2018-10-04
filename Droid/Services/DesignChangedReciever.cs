using System;
using Android.Content;
using Com.OneSignal.Abstractions;
using static Slink.Droid.ActionBroadcastReceiver;

namespace Slink.Droid.Services
{
    public class DesignChangedReciever : BroadcastReceiver
    {
        private IOnDataRecievedListener mListener;
        public Action<Intent> NotificationReceived;

        public DesignChangedReciever(IOnDataRecievedListener listener)
        {
            mListener = listener;
            mListener.RecieveData();
        }


        public override void OnReceive(Context context, Intent intent)
        {
            NotificationReceived?.Invoke(intent);
            var title = intent.GetStringExtra(Strings.InternalNotifications.notification_design_changed);
            //  if (String.IsNullOrEmpty(title)) return;
            if (mListener != null)
                mListener.RecieveData();

        }

    }
}
