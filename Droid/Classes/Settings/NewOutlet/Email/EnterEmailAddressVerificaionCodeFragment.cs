
using System;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace Slink.Droid
{
    public class EnterEmailAddressVerificaionCodeFragment : BaseFragment
    {
        public string PhoneNumber;
        EditText PhoneNumberTextField;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.EnterPhoneNumberVerificationCode, container, false);

            PhoneNumberTextField = view.FindViewById<EditText>(Resource.Id.PhoneNumberTextField);
            PhoneNumberTextField.InputType = Android.Text.InputTypes.ClassNumber;

            var titleTextView = view.FindViewById<TextView>(Resource.Id.TitleTextView);
            titleTextView.Text = "Verify Email Address";

            var messageTextView = view.FindViewById<TextView>(Resource.Id.MessageTextView);
            messageTextView.Text = "We've sent you an email!";

            var progressButton = view.FindViewById<Button>(Resource.Id.ProgressButton);
            progressButton.Click += async (sender, e) =>
            {
                string emailAddress = (string)Transporter.SharedInstance.GetObjectForKey("email");
                string code = PhoneNumberTextField.Text.Trim();

                bool allFieldsValid = ValidateAllFields();
                if (!allFieldsValid) return;

                ShowHud(Strings.Hud.please_wait);

                try
                {
                    var result = await WebServices.EmailController.UseValidationCode(emailAddress, code);
                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var outlet = new Outlet();
                        outlet.Handle = emailAddress;
                        outlet.Type = Outlet.outlet_type_email;
                        outlet.Name = emailAddress;

                        var outletSaved = RealmServices.SaveOutlet(outlet);
                        if (outletSaved)
                        {
                            var activity = Activity as BaseActivity;
                            activity?.PopFragmentOverUntil(typeof(MyOutletsRecyclerViewFragment));
                        }
                        else
                        {
                            ShowDuplicateEntryAlert();
                        }
                    }
                    else
                    {
                        PhoneNumberTextField.SetInvalid(Resources);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }

                HideHud();

            };


            return view;
        }
        void ShowDuplicateEntryAlert()
        {
            var builder = new AlertDialog.Builder(Activity);
            builder.SetTitle(Strings.Basic.error);
            builder.SetMessage(Strings.Alerts.outlet_duplicate_entry);
            builder.SetPositiveButton(Strings.Basic.ok, (senderAlert, args) => { });
            builder.Show();
        }
        bool ValidateAllFields()
        {
            string code = PhoneNumberTextField.Text.Trim();

            bool allFieldsValid = true;

            if (String.IsNullOrEmpty(code))
            {
                PhoneNumberTextField.SetInvalid(Resources);
                allFieldsValid = false;
            }

            return allFieldsValid;
        }
    }
}
