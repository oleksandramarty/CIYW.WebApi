using CIYW.Const.Enum;
using CIYW.Const.Errors;
using CIYW.Kernel.Exceptions;

namespace CIYW.Kernel.Extensions;

public static class EntityExtension
{
    public static void HasAccess<TEntity>(TEntity entity, Guid userId)
    {
        var propertyInfo = typeof(TEntity).GetProperty("UserId");

        if (propertyInfo != null)
        {
            var entityUserId = (Guid)propertyInfo.GetValue(entity);
            
            if (entityUserId != userId)
            {
                throw new LoggerException(ErrorMessages.Forbidden, 403, userId);
            }
            
            return;
        }

        throw new InvalidOperationException($"Type {typeof(TEntity).Name} does not have a UserId property.");
    }
}