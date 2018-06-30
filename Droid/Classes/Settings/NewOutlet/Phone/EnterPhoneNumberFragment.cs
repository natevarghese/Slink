using System;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Slink.Droid
{
    public class EnterPhoneNumberFragment : BaseFragment
    {
        EditText PhoneNumberTextField;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.EnterPhoneNumber, container, false);

            var titleTextView = view.FindViewById<TextView>(Resource.Id.TitleTextField);
            titleTextView.Text = "Add Phone Number";

            PhoneNumberTextField = view.FindViewById<EditText>(Resource.Id.PhoneNumberTextField);
            PhoneNumberTextField.Hint = Strings.Basic.phone_placeholder;
            PhoneNumberTextField.InputType = Android.Text.InputTypes.ClassPhone;

            var progressButton = view.FindViewById<Button>(Resource.Id.ProgressButton);
            progressButton.Click += async (sender, e) =>
            {
                string phoneNumber = StringUtils.RemoveNonIntegers(PhoneNumberTextField.Text.Trim());
                phoneNumber = StringUtils.RemoveLeadingSymbol(phoneNumber, "1");

                bool allFieldsValid = ValidateAllFields();
                if (!allFieldsValid) return;

                ShowHud(Strings.Hud.please_wait);

                try
                {
                    var result = await TwilioServices.StartPhoneVerificationAsync(phoneNumber);
                    if (result != null && result.success && result.is_cellphone)
                    {
                        var frag = new EnterPhoneNumberVerificaionCodeFragment();
                        frag.PhoneNumber = phoneNumber;
                        ((BaseActivity)Activity).AddFragmentOver(frag);
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
            string phoneNumber = StringUtils.RemoveNonIntegers(PhoneNumberTextField.Text.Trim());

            bool allFieldsValid = true;

            if (String.IsNullOrEmpty(phoneNumber) || phoneNumber.Length < 10 || phoneNumber.Length > 11)
            {
                PhoneNumberTextField.SetInvalid(Resources);
                allFieldsValid = false;
            }

            return allFieldsValid;
        }
    }
}
