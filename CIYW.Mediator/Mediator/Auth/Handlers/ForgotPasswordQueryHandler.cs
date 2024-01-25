using System.Text.RegularExpressions;
using AutoMapper;
using CIYW.Const.Errors;
using CIYW.Domain.Models.Common;
using CIYW.Domain.Models.Users;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Auth.Requests;
using CIYW.Mediator.Mediator.Common;
using CIYW.Mediator.Validators.Auth;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace CIYW.Mediator.Mediator.Auth.Handlers;

public class ForgotPasswordQueryHandler: UserEntityValidatorHelper, IRequestHandler<ForgotPasswordQuery>
{
    private readonly IGenericRepository<User> userRepository;
    private readonly IGenericRepository<RestorePassword> restorePasswordRepository;
    private IConfiguration configuration;
    
    public ForgotPasswordQueryHandler(
        IGenericRepository<User> userRepository,
        IGenericRepository<RestorePassword> restorePasswordRepository,
        IConfiguration configuration,
        IMapper mapper,
        ICurrentUserProvider currentUserProvider,
        IEntityValidator entityValidator): base(mapper, entityValidator, currentUserProvider)
    {
        this.userRepository = userRepository;
        this.restorePasswordRepository = restorePasswordRepository;
        this.configuration = configuration;
    }
    
    public async Task Handle(ForgotPasswordQuery query, CancellationToken cancellationToken)
    {
        this.ValidateRequest<ForgotPasswordQuery>(query, () => new ForgotPasswordQueryValidator());

        User user = await this.userRepository.GetByPropertyAsync(u =>
            u.Login == query.Login &&
            u.Email == query.Email &&
            u.PhoneNumber == query.Phone, cancellationToken);

        this.ValidateExist<User, Guid?>(user, user?.Id);

        DateTime dateNow = DateTime.UtcNow;

        if (user.Restored.HasValue && user.Restored.Value > dateNow.AddHours(-4))
        {
            throw new LoggerException(ErrorMessages.TryRestoreALittleLater, 409);
        }

        user.Restored = dateNow;

        Regex rgx = new Regex("[^a-zA-Z0-9]");

        string origin = this.configuration.GetSection("OriginUrl")?.Value ?? string.Empty;

        RestorePassword restore = new RestorePassword
        {
            Id = Guid.NewGuid(),
            Created = DateTime.UtcNow,
            UserId = user.Id,
            Url = StringExtension.GenerateRandomString(50).ToUpper()
        };

        //TODO EMAIL SERVICE SEND EMAIL WITH url token
        var urlToken = $"{origin}/auth/restore/{rgx.Replace($"{restore.Id}{restore.UserId}{restore.Url}", "")}";

        await this.userRepository.UpdateAsync(user, cancellationToken);
        await this.restorePasswordRepository.AddAsync(restore, cancellationToken);
    }
}