using System;
using Foundation;

namespace Slink.iOS
{
    public static class WebUtils
    {
        public static void ClearCookies()
        {
            var storage = NSHttpCookieStorage.SharedStorage;
            foreach (NSHttpCookie cookie in storage.Cookies)
                storage.DeleteCookie(cookie);
        }

        public static void ClearCache()
        {
            NSUrlCache.SharedCache.RemoveAllCachedResponses();
        }
    }
}
