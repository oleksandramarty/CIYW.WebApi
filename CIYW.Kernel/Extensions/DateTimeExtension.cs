namespace CIYW.Kernel.Extensions;

public static class DateTimeExtension
{
    public static double ConvertToUnixTimestamp(this DateTime date)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = date.ToUniversalTime() - origin;
        return Math.Floor(diff.TotalMilliseconds);
    }
}