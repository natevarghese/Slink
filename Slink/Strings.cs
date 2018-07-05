using System;
using Amazon.S3.Model;
namespace Slink
{
    public static class Strings
    {
        public class Basic
        {
            public static string ok = "OK";
            public static string yes = "Yes";
            public static string no = "No";
            public static string error = "Error";
            public static string cancel = "Cancel";
            public static string settings = "Settings";
            public static string new_card = "New Card";
            public static string your_name = "Your Name";
            public static string title = "Title";
            public static string company_name = "Company Name";
            public static string first_name = "First Name";
            public static string last_name = "Last Name";
            public static string phone_placeholder = "XXX-XXX-XXXX";
            public static string verification_code = "Verification Code";
            public static string delete = "Delete";
            public static string delete_card = "Delete Card";
            public static string skip = "Skip";
            public static string lets_go = "Let's Go!";
            public static string email_address = "Email Address";
        }
        public class Onboarding
        {
            public static string hi_im_slink = "Hi! I'm Slink!";
            public static string im_here_to_help = "I'm here to help you get connected";
            public static string begin_by = "Begin by creating cards";
            public static string share_with_people_nearby = "Tap the share button to share your cards with people nearby";
            public static string get_notified = "Get notified when people share with you";
            public static string add_a_name = "Don't forget to give your card a name";
            public static string add_your_name_and_image = "Add your name and picture";
            public static string default_name = "John Doe";
            public static string color_border = "Change the border color";
            public static string add_phone_number = "Add your phone number";
            public static string add_social_media = "Even your favorite social media accounts";
            public static string social = "Social";
            public static string tap_to_share = "Tap to share";
        }
        public class Sharing
        {
            public static string tap_to_share = "Tap to share";
            public static string select_at_least_one = "Select at least one outlet";
            public static string location_permission_necessary = "Location Permission Necessary. \n Go to Settings";
            public static string could_not_share_card = "Could not share card";
            public static string authenticating = "Authenticating";
        }
        public class Discover
        {
            public static string item_presented = "Swipe to add to your roladex";
        }
        public class Permissions
        {
            public static string photos_disabled = "Photos Disabled";
            public static string location_disabled = "Location Disabled";
        }

        public class Hud
        {
            public static string loading = "Loading";
            public static string please_wait = "Please Wait";
        }

        public class Alerts
        {
            public static string invalid_login = "Invalid Login";
            public static string please_try_again_later = "Please Try Again Later";
            public static string server_reachability = "We are having issues connecting to our servers";
            public static string user_email_already_exists = "That email is already taken, please login with that one";
            public static string card_missing_name = "Every card must have a name";
            public static string card_duplicate_name = "You already have a card with that name, try another one";
            public static string outlet_duplicate_entry = "You already have an outlet with that handle and type";
            public static string card_missing_outlets = "You must add at least one outlet";
            public static string select_image_source = "Select Image Source";
            public static string user_facebook_image = "Use Facebook image";
            public static string select_from_gallery = "Select from gallery";
        }

        public class InternalNotifications
        {
            public static string notification_invalid_token = "InvalidTokenNotificaion";
            public static string notification_scan_button_clicked = "ScanButtonClickedNotificaion";
            public static string notification_show_auto_complete_suggestions = "ShowAutoCompleteSuggestions";
            public static string notification_table_row_editing_changed = "TableViewRowEditingChangedNotification";
            public static string notification_card_editing_changed = "TableViewCardEditingChangedNotification";
            public static string notification_no_outlets_tapped = "NoOutletsTappedNotification";
            public static string notification_design_changed = "DesignChangedNotification";
            public static string notification_companyN = "CollectionViewTappedNotification";
            public static string notification_collection_view_tapped = "CollectionViewTappedNotification";
            public static string notification_card_user_image_clicked = "CardUserImageClickedNotification";
            public static string notification_company_logo_image_clicked = "CardCompanyLogoImageClickedNotification";
        }
        public class PushNotificaionKeys
        {
            public static string nearby_broadcast = "nearby_broadcast";
        }
        public class PersistantKeys
        {
            public static string persistant_key_user_id = "UserId";
            public static string persistant_key_email = "Email";
            public static string persistant_key_first_name = "FirstName";
            public static string persistant_key_last_name = "LastName";
            public static string persistant_key_user_password = "UserPassword";
            public static string persistant_key_access_token = "AccessToken";
            public static string persistant_key_refresh_token = "RefreshToken";
            public static string persistant_key_access_expiry = "AccessExpiry";
            public static string persistant_key_access_scope = "AccessScope";
            public static string persistant_key_facebook_access_token = "FacebookAccessToken";
            public static string persistant_key_facebook_access_expiry = "FacebookAccessExpiry";
            public static string persistant_key_push_token = "PushToken";
            public static string persistant_key_discover_count = "DiscoverCount";
            public static string persistant_key_design_type = "DesignType";

        }
        public class ImageFileNames
        {
            public static string no_profile = "NoProfile";
        }
        public class ImageUrls
        {
            public static string default_card = "http://images.nationalgeographic.com/wpf/media-live/photos/000/202/cache/tiger-face-snarl-hiss-close-up_20246_990x742.jpg";//todo
            public static string outlet_facebook = "https://s3.us-east-2.amazonaws.com/slinka/Outlets/1.0/facebook.png";
            public static string outlet_email = "http://www.iconsdb.com/icons/preview/orange/email-14-xxl.png";//todo
            public static string outlet_google_plus = "";
            public static string outlet_phone = "https://s3.us-east-2.amazonaws.com/slinka/Outlets/1.0/phone.png";
            public static string outlet_linkedIn = "";
        }
        public class DesignTypes
        {
            public static string design_type_none = "None";
            public static string design_type_flying_colors = "FlyingColors";
            public static string design_type_flying_lights = "FlyingLights";
        }

        public class PageTitles
        {
            public static string page_title_select_type = "Select Type";
            public static string page_title_select_an_outlet = "Select an Outlet";
            public static string page_title_github = "Github";
            public static string page_title_linkedin = "LinkedIn";
            public static string page_title_pinterest = "Pinterest";
            public static string page_title_instagram = "Instagram";
        }
        public class TableViewFooters
        {
            public static string table_view_footer_create_new_outlet = "Create New Outlet +";
        }
    }
}
