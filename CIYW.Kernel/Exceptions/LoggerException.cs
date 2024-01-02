using CIYW.Kernel.Errors;

namespace CIYW.Kernel.Exceptions;

public class LoggerException: Exception
{
    public LoggerException(
        string message,
        int _statusCode,
        Guid? _userId = null,
        IReadOnlyCollection<InvalidFieldInfo> invalidFields = null,
        string _payload = null) :
        base(message)
    {
        statusCode = _statusCode;
        userId = _userId;
        payload = _payload;
    }

    public int statusCode { get; set; }
    public Guid? userId { get; set; }
    public string payload { get; set; }
    public IReadOnlyCollection<InvalidFieldInfo> invalidFields { get; set; }
    public ErrorMessage ToErrorMessage()
    {
        return new ErrorMessage(Message, statusCode, invalidFields);
    }
}