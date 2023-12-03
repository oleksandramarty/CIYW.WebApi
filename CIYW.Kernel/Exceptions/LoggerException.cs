using CIYW.Kernel.Errors;

namespace CIYW.Kernel.Exceptions;

public class LoggerException: Exception
{
    public LoggerException(
        string message,
        int _statusCode,
        Guid? _userId,
        string _entityType,
        string _payload = null) :
        base(message)
    {
        statusCode = _statusCode;
        userId = _userId;
        payload = _payload;
        entityType = _entityType;
    }

    public LoggerException(
        Exception ex,
        int _statusCode,
        Guid? _userId,
        string _entityType,
        string _payload = null) : base(ex.Message)
    {
        statusCode = _statusCode;
        userId = _userId;
        payload = _payload;
        entityType = _entityType;
    }

    public int statusCode { get; set; }
    public Guid? userId { get; set; }
    public string payload { get; set; }
    public string entityType { get; set; }
    public ErrorMessage ToErrorMessage()
    {
        return new ErrorMessage(Message, statusCode);
    }

}