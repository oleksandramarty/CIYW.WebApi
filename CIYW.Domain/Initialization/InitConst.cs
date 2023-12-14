namespace CIYW.Domain.Initialization;

public class InitConst
{
    public static Guid UserRoleId
    {
        get => new Guid("C5B2F100-1711-44F2-8338-83822D22EB06");
    }

    public static Guid AdminUserId
    {
        get => new Guid("851A3339-BEDA-4313-A5FA-A03266B85253");
    }
    
    public static Guid FreeTariffId
    {
        get => new Guid("F76B2192-D512-4E7D-9013-70EEB0BE5518");
    }
}