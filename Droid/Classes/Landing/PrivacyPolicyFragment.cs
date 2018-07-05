namespace Slink.Droid
{
    public class PrivacyPolicyFragment : BaseWebViewFragment
    {
        public override string GetUrl()
        {
            return NotSensitive.SystemUrls.privacy_policy;
        }
    }
}
