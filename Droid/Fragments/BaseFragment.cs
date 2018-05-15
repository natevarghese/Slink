using System;
using Android.App;
using Android.Content;
using Android.Support.V7.App;

namespace Slink.Droid
{
    public class BaseFragment : Android.Support.V4.App.Fragment
    {
        ProgressDialog Progress;

        public virtual void ShowHud(string str)
        {
            if (Progress == null)
            {
                Progress = new ProgressDialog(Activity);
                Progress.Indeterminate = true;
                Progress.SetProgressStyle(Android.App.ProgressDialogStyle.Spinner);
                Progress.SetMessage(str);
                Progress.SetCancelable(false);
            }
            Progress.Show();
        }

        public virtual void HideHud()
        {
            Progress?.Dismiss();
            Progress = null;
        }

        public void SetOverFragment(string className)
        {
            var intent = new Intent(LandingActivity.UpdateOverFragmentBroadcastReceiverKey);
            intent.PutExtra(LandingActivity.UpdateOverFragmentBroadcastReceiverKeyFileName, className);
            Context.SendBroadcast(intent);
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                Progress?.Dispose();
                Progress = null;
            }
        }
    }
}
