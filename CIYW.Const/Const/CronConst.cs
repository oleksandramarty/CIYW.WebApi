namespace CIYW.Const.Const;

public class CronConst
{
    public const string Cron1am = "0 1 * * *";
    public const string Cron2am = "0 2 * * *";
    public const string Cron3am = "0 3 * * *";
    public const string Cron4am = "0 4 * * *";
    public const string CronEveryMin = "* * * * *";
    public const string CronEvery5mins = "*/5 * * * *";
    public const string CronEvery30mins = "*/30 * * * *";
    public const string CronEvery10mins = "*/10 * * * *";
    public const string CronEvery50thMins = "*/50 * * * *";
    public const string CronEvery30minsFrom8To20Hours = "*/30 5-17 * * *";
    public const string CronEveryAt20Hours = "0 17 * * *";
    public const string CronEveryHour = "10 * * * *";
    public const string CronEvery12hours = "20 */12 * * *";
    public const string CronAt15min = "15 * * * *";        
    public const string CronEveryThirdMinInHour = "3 0-23 * * *";        
    public const string CronEveryThirdMinIn4Hour = "3 */4 * * *";        
    public const string CronAt20H35MOnTuesday = "35 20 * * 2";   
}