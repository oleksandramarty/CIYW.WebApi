using System.Reflection;
using Autofac;
using CIYW.Mediator.Mediatr.Auth.Handlers;
using CIYW.Mediator.Mediatr.Balance.Requests;
using CIYW.Mediator.Mediatr.Currency.Handlers;
using CIYW.Mediator.Mediatr.Dictionary.Handlers;
using CIYW.Mediator.Mediatr.Invoice.Handlers;
using CIYW.Mediator.Mediatr.Note.Handlers;
using CIYW.Mediator.Mediatr.Tariff.Handlers;
using CIYW.Mediator.Mediatr.Users.Handlers;
using MediatR;

namespace CIYW.Mediator;

public class MediatrModule : Autofac.Module
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
        #endregion

        #region Invoice
        builder.RegisterAssemblyTypes(typeof(CreateInvoiceCommandHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        builder.RegisterAssemblyTypes(typeof(UpdateInvoiceCommandHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<>));
        builder.RegisterAssemblyTypes(typeof(DeleteInvoiceCommandHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<>));
        builder.RegisterAssemblyTypes(typeof(UserInvoicesQueryHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        builder.RegisterAssemblyTypes(typeof(UserMonthlyInvoicesQueryHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        #endregion

        #region Note
        builder.RegisterAssemblyTypes(typeof(CreateNoteCommandHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        builder.RegisterAssemblyTypes(typeof(UpdateNoteCommandHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        builder.RegisterAssemblyTypes(typeof(DeleteNoteCommandHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<>));
        #endregion

        #region Tariff
        builder.RegisterAssemblyTypes(typeof(TariffQueryHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        #endregion

        #region User
        builder.RegisterAssemblyTypes(typeof(CreateUserCommandHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<>));
        builder.RegisterAssemblyTypes(typeof(CurrentUserQueryHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        #endregion

        #region Balance
        builder.RegisterAssemblyTypes(typeof(UserBalanceQuery).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        #endregion

        #region Dictionary
        builder.RegisterAssemblyTypes(typeof(DictionaryTypeQueryHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        builder.RegisterAssemblyTypes(typeof(DictionaryQueryHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
        #endregion
    }
}