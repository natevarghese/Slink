using System;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Slink.Droid
{
    public class EnterEmailAddressFragment : BaseFragment
    {
        EditText PhoneNumberTextField;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.EnterPhoneNumber, container, false);

            var titleTextView = view.FindViewById<TextView>(Resource.Id.TitleTextField);
            titleTextView.Text = "Add Email Address";

            PhoneNumberTextField = view.FindViewById<EditText>(Resource.Id.PhoneNumberTextField);
            PhoneNumberTextField.Hint = Strings.Basic.email_address;
            PhoneNumberTextField.InputType = Android.Text.InputTypes.TextVariationEmailAddress;

            var progressButton = view.FindViewById<Button>(Resource.Id.ProgressButton);
            progressButton.Click += async (sender, e) =>
            {
                string emailAddress = PhoneNumberTextField.Text.Trim();

                bool allFieldsValid = ValidateAllFields();
                if (!allFieldsValid) return;

                ShowHud(Strings.Hud.please_wait);

                try
                {
                    var result = await WebServices.EmailController.SendValidationCode(emailAddress);
                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Transporter.SharedInstance.SetObject("email", emailAddress);
                        ((BaseActivity)Activity).AddFragmentOver(new EnterEmailAddressVerificaionCodeFragment());
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

        bool ValidateAllFields()
        {
            string emailAddress = PhoneNumberTextField.Text.Trim();

            bool allFieldsValid = true;

            if (String.IsNullOrEmpty(emailAddress) || !StringUtils.IsValidEmail(emailAddress))
            {
                PhoneNumberTextField.SetInvalid(Resources);
                allFieldsValid = false;
            }

            return allFieldsValid;
        }
    }
}
