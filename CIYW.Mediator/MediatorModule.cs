using System.Reflection;
using Autofac;
using CIYW.Mediator.Mediator.Auth.Handlers;
using CIYW.Mediator.Mediator.Balance.Requests;
using CIYW.Mediator.Mediator.Category.Handlers;
using CIYW.Mediator.Mediator.Currency.Handlers;
using CIYW.Mediator.Mediator.Dictionary.Handlers;
using CIYW.Mediator.Mediator.FIle.Handlers;
using CIYW.Mediator.Mediator.Invoice.Handlers;
using CIYW.Mediator.Mediator.Note.Handlers;
using CIYW.Mediator.Mediator.Tariff.Handlers;
using CIYW.Mediator.Mediator.Users.Handlers;
using MediatR;

namespace CIYW.Mediator;

public class MediatorModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
            .AsImplementedInterfaces();

        #region Auth
        builder.RegisterAssemblyTypes(typeof(AuthLoginQueryHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        builder.RegisterAssemblyTypes(typeof(AuthLogoutQueryHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<>));
        builder.RegisterAssemblyTypes(typeof(ChangePasswordCommandHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<>));
        builder.RegisterAssemblyTypes(typeof(CheckTemporaryPasswordQueryHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        builder.RegisterAssemblyTypes(typeof(ForgotPasswordQueryHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<>));
        builder.RegisterAssemblyTypes(typeof(ResetPasswordCheckAccessQueryHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<>));
        builder.RegisterAssemblyTypes(typeof(RestorePasswordCommandHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<>));
        #endregion

        #region Currency
        builder.RegisterAssemblyTypes(typeof(CurrencyQueryHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        builder.RegisterAssemblyTypes(typeof(CreateCurrencyCommandHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        builder.RegisterAssemblyTypes(typeof(UpdateCurrencyCommandHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        #endregion

        #region Invoice
        builder.RegisterAssemblyTypes(typeof(CreateInvoiceCommandHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        builder.RegisterAssemblyTypes(typeof(UpdateInvoiceCommandHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        builder.RegisterAssemblyTypes(typeof(DeleteInvoiceCommandHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<>));
        builder.RegisterAssemblyTypes(typeof(UserInvoicesQueryHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        builder.RegisterAssemblyTypes(typeof(UserMonthlyInvoicesQueryHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        builder.RegisterAssemblyTypes(typeof(GetInvoiceByIdQueryHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        #endregion

        #region Note
        builder.RegisterAssemblyTypes(typeof(CreateNoteCommandHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        builder.RegisterAssemblyTypes(typeof(UpdateNoteCommandHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        builder.RegisterAssemblyTypes(typeof(DeleteNoteCommandHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<>));
        #endregion

        #region Tariff
        builder.RegisterAssemblyTypes(typeof(TariffQueryHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        builder.RegisterAssemblyTypes(typeof(CreateTariffCommandHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        builder.RegisterAssemblyTypes(typeof(UpdateTariffCommandHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        #endregion

        #region User
        builder.RegisterAssemblyTypes(typeof(CreateUserCommandHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<>));
        builder.RegisterAssemblyTypes(typeof(CurrentUserQueryHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        builder.RegisterAssemblyTypes(typeof(UsersQueryHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        #endregion

        #region Balance
        builder.RegisterAssemblyTypes(typeof(UserBalanceQuery).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        #endregion

        #region Dictionary
        builder.RegisterAssemblyTypes(typeof(DictionaryTypeQueryHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        builder.RegisterAssemblyTypes(typeof(DictionaryQueryHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        builder.RegisterAssemblyTypes(typeof(DictionaryEnumQueryHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        #endregion

        #region Category
        builder.RegisterAssemblyTypes(typeof(CreateCategoryCommandHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        builder.RegisterAssemblyTypes(typeof(UpdateCategoryCommandHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        #endregion

        #region Image
        builder.RegisterAssemblyTypes(typeof(UserImageQueryHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        builder.RegisterAssemblyTypes(typeof(UsersImagesQueryHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        builder.RegisterAssemblyTypes(typeof(CreateImageCommandHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        builder.RegisterAssemblyTypes(typeof(UpdateImageCommandHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<>));
        builder.RegisterAssemblyTypes(typeof(DeleteImageCommandHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<>));
        #endregion
    }
}