namespace CIYW.Kernel.Errors;

public class InvalidFieldInfo
{
    public InvalidFieldInfo(string propertyName, string errorMessage)
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
    }

    public string PropertyName { get; }
    public string ErrorMessage { get; }
}