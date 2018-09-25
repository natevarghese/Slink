using System;

using UIKit;

namespace Slink.iOS
{
    public partial class EnterPhoneNumberViewController : BaseLandingViewController
    {
        public EnterPhoneNumberViewController(IntPtr handle) : base(handle) { }
        public EnterPhoneNumberViewController() : base("EnterPhoneNumberViewController") { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            RemoveBackBarButtonTitle();

            PhoneNumberTextField.KeyboardType = UIKeyboardType.PhonePad;
            PhoneNumberTextField.Placeholder = Strings.Basic.phone_placeholder;
            PhoneNumberTextField.AutocorrectionType = UITextAutocorrectionType.No;
            PhoneNumberTextField.BecomeFirstResponder();
            PhoneNumberTextField.EditingDidEndOnExit += (sender, e) =>
            {
                PhoneNumberTextField.ResignFirstResponder();
            };
            SetProgressButton(ValidateButton);
        }

        protected override bool ValidateAllFields()
        {
            string phoneNumber = StringUtils.RemoveNonIntegers(PhoneNumberTextField.Text.Trim());

            bool allFieldsValid = true;

            if (String.IsNullOrEmpty(phoneNumber) || phoneNumber.Length < 10 || phoneNumber.Length > 11)
            {
                PhoneNumberTextField.SetInvalid();
                allFieldsValid = false;
            }

            return allFieldsValid;
        }
        public override TPKeyboardAvoiding.TPKeyboardAvoidingScrollView GetScrollView()
        {
            return ScrollView;
        }

        async partial void ValidateButtonClicked(Foundation.NSObject sender)
        {
            View.EndEditing(true);

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
                    var vc = new EnterPhoneNumberVerificationCodeViewController();
                    vc.PhoneNumber = phoneNumber;
                    NavigationController.PushViewController(vc, true);
                }
                else
                {
                    PhoneNumberTextField.SetInvalid();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            HideHud();
        }
    }
}

