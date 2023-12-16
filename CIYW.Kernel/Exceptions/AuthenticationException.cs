using CIYW.Const.Errors;
using CIYW.Kernel.Errors;

namespace CIYW.Kernel.Exceptions;

public class AuthenticationException : LoggerException
{
    public AuthenticationException(string message, int _statusCode, Guid? _userId = null, string _payload = null) : base(message, _statusCode, _userId, _payload)
    {
    }
    
    new public ErrorMessage ToErrorMessage()
    {
        if(statusCode == 409)
        {
            return new ErrorMessage(ErrorMessages.UserBlocked, statusCode);
        }
        else // if(statusCode == 404)
        {
            return new ErrorMessage(ErrorMessages.WrongAuth, statusCode);
        }      
    }
}