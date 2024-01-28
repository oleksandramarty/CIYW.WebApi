using CIYW.Resources.Forms.Enum;
using CIYW.Resources.Forms.Models;

namespace CIYW.Resources.Forms.Definitions;

public static class DefinitionHelper
{
    public static FormModel GenerateFormModel(this FormTypeEnum type)
    {
        switch (type)
        {
            case FormTypeEnum.AUTH_LOGIN:
                return GenerateLoginAuthForm();
            default:
                return null;
        }
    }
    public static FormModel GenerateLoginAuthForm()
    {
        return new FormModel
        {
            Type = FormTypeEnum.AUTH_LOGIN,
            Title = "Login to Finance Control",
            SubmitButton = new FormButtonModel
            {
                Title = "Login"
            },
            Controls = new List<FormItemModel>
            {
                new FormItemModelBuilder("login", "Login")
                    .WithValidators(new FormItemValidatorModelBuilder().WithAtLeastOne("1:login-email-phone").Build()).Build(),
                new FormItemModelBuilder("email", "Email")
                    .WithValidators(new FormItemValidatorModelBuilder().WithAtLeastOne("1:login-email-phone").Build()).Build(),
                new FormItemModelBuilder("phone", "Phone")
                    .WithValidators(
                        new FormItemValidatorModelBuilder()
                            .WithPattern("/^\\d{10}$/")
                            .WithAtLeastOne("1:login-email-phone")
                            .Build()
                        ).Build(),
                new FormItemModelBuilder("password", "Password")
                    .WithInputType(FormItemInputType.Password)
                    .WithValidators(new FormItemValidatorModelBuilder().WithRequired().Build()).Build(),
                new FormItemModelBuilder("rememberMe")
                    .WithLabel("Remember me")
                    .WithDefaultValue(true)
                    .WithInputType(FormItemInputType.Checkbox).Build(),
            }
        };
    }
}