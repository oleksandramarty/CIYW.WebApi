using CIYW.Kernel.Errors;
using FluentValidation.Results;

namespace CIYW.Kernel.Extensions;

public static class FluentValidationExtension
{
    public static IReadOnlyCollection<InvalidFieldInfo> GetInvalidFieldInfo(this ValidationResult validationResult)
    {
        return validationResult.Errors
            .Select(e => new InvalidFieldInfo(e.PropertyName, e.ErrorMessage))
            .ToList();
    }

}