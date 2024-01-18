using System.Globalization;
using System.Runtime.CompilerServices;
using CIYW.Domain.Models;
using CIYW.Models.Responses.Base;

namespace CIYW.Kernel.Extensions;

public static class DateTimeExtension
{
    public static double ConvertToUnixTimestamp(this DateTime date)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = date.ToUniversalTime() - origin;
        return Math.Floor(diff.TotalMilliseconds);
    }

    public static DateTime GetStartOfTheMonth()
    {
        DateTime currentDate = DateTime.Today;
        return new DateTime(currentDate.Year, currentDate.Month, 1);
    }

    public static DateTime GetLocalTime(this DateTime date, int hours, int minutes)
    {
        return date.AddHours(hours).AddMinutes(minutes);
    }
    
    public static DateTime GetEndOfTheMonth()
    {
        DateTime currentDate = DateTime.Today;
        return new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month), 23, 59, 59);
    }
    public static string HumanizeModified(this BaseWithDateEntity entity)
    {
        return (entity.Updated ?? entity.Created).Humanize();
    }
    public static string HumanizeModified(this BaseWithDateEntityResponse entity)
    {
        return (entity.Updated ?? entity.Created).Humanize();
    }

    public static string Humanize(this DateTime? dateTime)
    {
        if (!dateTime.HasValue)
        {
            return string.Empty;
        }

        return dateTime.Value.GetLocalTime(2, 0).Humanize();
    }
    
    public static string Humanize(this DateTime dateTime)
    {
        TimeSpan timeAgo = DateTime.Now - dateTime.GetLocalTime(2, 0);

        if (timeAgo.TotalMinutes < 1)
            return "just now";

        if (timeAgo.TotalMinutes < 60)
            return $"{(int)timeAgo.TotalMinutes} minute{((int)timeAgo.TotalMinutes).GetPlural()} ago";

        if (timeAgo.TotalHours < 24)
            return $"{(int)timeAgo.TotalHours} hour{((int)timeAgo.TotalHours).GetPlural()} ago";

        if (timeAgo.TotalDays < 30)
            return $"{(int)timeAgo.TotalDays} day{((int)timeAgo.TotalDays).GetPlural()} ago";

        if (timeAgo.TotalDays < 365)
            return $"{(int)(timeAgo.TotalDays / 30)} month{((int)(timeAgo.TotalDays / 30)).GetPlural()} ago";

        return $"{(int)(timeAgo.TotalDays / 365)} year{((int)(timeAgo.TotalDays / 365)).GetPlural()} ago";
    }

    private static string GetPlural(this int temp)
    {
        return temp != 1 ? "s" : "";
    }
}