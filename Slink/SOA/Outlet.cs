
using System;
using Newtonsoft.Json.Linq;
using PCLStorage;
using Realms;

namespace Slink
{
    public class Outlet : RealmObject
    {
        public string Name { get; set; } //used for the rendering on the view
        public string Image { get; set; }
        public string Type { get; set; } //must be of static string outlet_type
        public string Handle { get; set; } //this is the unique identifier required for the outlet
        public Card Card { get; set; }
        public bool Locked { get; set; } //if true, is tied to the realm account. Can not remove this
        public bool AvailbleForAddition { get; set; } //used to show if another outlet of this type can be added to the user's account
        public bool Omitted { get; set; }
        public bool Deleted { get; set; }

        public static string outlet_type_facebook = "Facebook";
        public static string outlet_type_email = "Email";
        public static string outlet_type_phone = "Phone";
        public static string outlet_type_linkedIn = "LinkedIn";
        public static string outlet_type_twitter = "Twitter";
        public static string outlet_type_instagram = "Instagram";
        public static string outlet_type_pinterest = "Pinterest";
        public static string outlet_type_github = "Github";
        public static string outlet_type_website = "Website";
        public static string outlet_type_google = "Google";
        public static string outlet_image_version = "1.0";




        public string LocalURL
        {
            get
            {
                return FileSystem.Current.LocalStorage.Path + "/Outlets/" + Type.ToLowerInvariant() + ".png";
            }
            set { }
        }
        public string RemoteURL
        {
            get
            {
                return NotSensitive.SystemUrls.s3_url + "Outlets/" + outlet_image_version + "/" + Type.ToLowerInvariant() + ".png";
            }
            set { }
        }
        public JObject ToJObject()
        {
            var returnObj = new JObject();
            returnObj["name"] = Name;
            returnObj["type"] = Type;
            returnObj["handle"] = Handle;
            return returnObj;
        }
    }
}