using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slink
{
    public static class DateUtils
    {
		public static string GetRelativeDate(DateTime? Date)
		{
            if (Date == null || !Date.HasValue) return String.Empty;

            return GetRelativeDate(Date.Value.ToLocalTime());
		}
        public static string GetRelativeDate(DateTime Date)
        {
			const int SECOND = 1;
            const int MINUTE = 60 * SECOND;

            TimeSpan myTimeSpan = new TimeSpan(DateTime.Now.Ticks - Date.Ticks);
            double myDelta = Math.Abs(myTimeSpan.TotalSeconds);

			if (myDelta < 1 * MINUTE)
			{
				return (myTimeSpan.Seconds < 5) ? "moments ago" : myTimeSpan.Seconds + " seconds ago";
			}

            if (myDelta < 2 * MINUTE)
                return "a minute ago";

            if (myDelta < 45 * MINUTE)
                return myTimeSpan.Minutes + " minutes ago";

            if (myDelta < 90 * MINUTE)
                return "an hour ago";

            if ((DateTime.Today - Date.Date) < TimeSpan.FromDays(1))
                return "Today " + Date.ToString("h:mmtt").Replace("PM", "pm").Replace("AM", "am");

            if ((DateTime.Today - Date.Date) == TimeSpan.FromDays(1))
                return "Yesterday " + Date.ToString("h:mmtt").Replace("PM", "pm").Replace("AM", "am");

            if ((DateTime.Today - Date.Date) < TimeSpan.FromDays(7))
                return Date.ToString("dddd h:mmtt").Replace("PM", "pm").Replace("AM", "am");

            if ((DateTime.Today - Date.Date) < TimeSpan.FromDays(365))
                return Date.ToString("MMMM dd h:mmtt").Replace("PM", "pm").Replace("AM", "am");

            return Date.ToString("MMMM dd, yyyy h:mmtt").Replace("PM", "pm").Replace("AM", "am");
        }
    }
}
