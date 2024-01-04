namespace CIYW.Kernel.Extensions;

public static class BoolExtension
{
    public static bool GetRandomBool()
    {
        Random random = new Random();
        return random.Next(2) == 0;
    }
}