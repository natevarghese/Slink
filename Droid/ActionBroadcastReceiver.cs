using System;
using Android.Content;

namespace Slink.Droid
{
    public class ActionBroadcastReceiver : BroadcastReceiver
    {
        public Action<Intent> NotificationReceived;

        public override void OnReceive(Context context, Intent intent)
        {
            NotificationReceived?.Invoke(intent);
        }
    }
}
