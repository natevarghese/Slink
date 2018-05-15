using System;
using System.Text;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Slink
{
    public class AppCenterManager : BaseQueue
    {
        public static bool Enabled = true;
        public override void Start()
        {
            if (Running) return;
            Running = true;


#if DEBUG
            Enabled = false;
#endif

            Analytics.SetEnabledAsync(Enabled);

            Crashes.SetEnabledAsync(Enabled);
            Crashes.ShouldAwaitUserConfirmation = () => { return false; };
            Crashes.ShouldProcessErrorReport = (ErrorReport report) => { return true; };

            Crashes.SentErrorReport += (sender, e) =>
            {
                // Your code, e.g. to hide the custom UI.
            };
            Crashes.FailedToSendErrorReport += (sender, e) =>
            {
                // Your code goes here.
            };
        }

        public static void Report(string message)
        {
            if (String.IsNullOrEmpty(message)) return;

            System.Diagnostics.Debug.WriteLine(message);
            Analytics.TrackEvent(message);
        }

        public static void Report(Exception e)
        {
            if (e == null) return;

            System.Diagnostics.Debug.WriteLine(e.Message);
            Crashes.TrackError(e);
        }
    }
}
