using System;
using System.Linq;
using UIKit;

namespace Slink.iOS
{
    public partial class EnterEmailAddressVerificationCodeViewController : BaseLandingViewController
    {
        public EnterEmailAddressVerificationCodeViewController() : base("EnterEmailAddressVerificationCodeViewController") { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            RemoveBackBarButtonTitle();

            VerificationCodeTextField.KeyboardType = UIKeyboardType.NumberPad;
            VerificationCodeTextField.Placeholder = Strings.Basic.verification_code;
            VerificationCodeTextField.AutocorrectionType = UITextAutocorrectionType.No;
            VerificationCodeTextField.BecomeFirstResponder();
            VerificationCodeTextField.EditingDidEndOnExit += (sender, e) =>
            {
                VerificationCodeTextField.ResignFirstResponder();
            };

            SetProgressButton(NextButton);
        }

        protected override bool ValidateAllFields()
        {
            string code = VerificationCodeTextField.Text.Trim();

            bool allFieldsValid = true;

            if (String.IsNullOrEmpty(code))
            {
                VerificationCodeTextField.SetInvalid();
                allFieldsValid = false;
            }

            return allFieldsValid;
        }
        public override TPKeyboardAvoiding.TPKeyboardAvoidingScrollView GetScrollView()
        {
            return ScrollView;
        }
        async partial void NextButtonClicked(Foundation.NSObject sender)
        {
            View.EndEditing(true);

            string emailAddress = (string)Transporter.SharedInstance.GetObjectForKey("email");
            string code = VerificationCodeTextField.Text.Trim();

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
                        var popToViewController = NavigationController.ViewControllers.Where(c => c.GetType() == typeof(MyOutletsViewController)).First();
                        NavigationController.PopToViewController(popToViewController, true);
                    }
                    else
                    {
                        ShowDuplicateEntryAlert();
                    }
                }
                else
                {
                    VerificationCodeTextField.SetInvalid();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            HideHud();
        }
        void ShowDuplicateEntryAlert()
        {
            UIAlertController AlertController = UIAlertController.Create(Strings.Basic.error, Strings.Alerts.outlet_duplicate_entry, UIAlertControllerStyle.Alert);
            AlertController.AddAction(UIAlertAction.Create(Strings.Basic.ok, UIAlertActionStyle.Default, null));
            PresentViewController(AlertController, true, null);
        }
    }
}