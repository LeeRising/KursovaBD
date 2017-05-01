using System;

namespace KursovaBD
{
    class RelativeTime
    {
        internal static string _RelativeTime(DateTime last_online)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(DateTime.Now.Ticks - last_online.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "Last online:1 second ago" : "Last online:"+ts.Seconds + " seconds ago";

            if (delta < 2 * MINUTE)
                return "Last online:a minute ago";

            if (delta < 45 * MINUTE)
                return "Last online:" + ts.Minutes + " minutes ago";

            if (delta < 90 * MINUTE)
                return "Last online:an hour ago";

            if (delta < 24 * HOUR)
                return "Last online:" + ts.Hours + " hours ago";

            if (delta < 48 * HOUR)
                return "Last online:yesterday";

            if (delta < 30 * DAY)
                return "Last online:" + ts.Days + " days ago";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "Last online:1 month ago" : "Last online:" + months + " months ago";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "Last online:1 year ago" : "Last online:" + years + " years ago";
            }
        }
    }
}
