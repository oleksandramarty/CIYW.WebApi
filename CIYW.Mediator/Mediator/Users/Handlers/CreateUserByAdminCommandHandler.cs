﻿using AutoMapper;
using CIYW.Const.Const;
using CIYW.Const.Errors;
using CIYW.Const.Providers;
using CIYW.Domain.Initialization;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Responses.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CIYW.Mediator.Mediator.Users.Handlers;

public class CreateUserByAdminCommandHandler: IRequestHandler<CreateUserByAdminCommand, MappedHelperResponse<UserResponse, User>>
{
    private readonly IMapper mapper;
    private readonly IEntityValidator entityValidator;
    private readonly IAuthRepository authRepository;
    private readonly UserManager<User> userManager;

    public CreateUserByAdminCommandHandler(
        IMapper mapper,
        IEntityValidator entityValidator, 
        IAuthRepository authRepository, 
        UserManager<User> userManager)
    {
        this.mapper = mapper;
        this.entityValidator = entityValidator;
        this.authRepository = authRepository;
        this.userManager = userManager;
    }
    
        public async Task<MappedHelperResponse<UserResponse, User>> Handle(CreateUserByAdminCommand command, CancellationToken cancellationToken)
    {
        if (!command.Email.TrimWhiteSpaces().Equals(command.ConfirmEmail.TrimWhiteSpaces()))
        {
            throw new LoggerException(ErrorMessages.EmailsDoesntMatch, 409);
        }
        
        if (!command.Password.TrimWhiteSpaces().Equals(command.ConfirmPassword.TrimWhiteSpaces()))
        {
            throw new LoggerException(ErrorMessages.PasswordsDoesntMatch, 409);
        }
        
        if (!command.IsAgree)
        {
            throw new LoggerException(ErrorMessages.AgreeBeforeSignIn, 409);
        }
        
        await this.entityValidator.ValidateExistParamAsync<User>(u => u.Email == command.Email, String.Format(ErrorMessages.UserWithParamExist, DefaultConst.Email), cancellationToken);
        await this.entityValidator.ValidateExistParamAsync<User>(u => u.PhoneNumber == command.Phone, String.Format(ErrorMessages.UserWithParamExist, DefaultConst.Phone), cancellationToken);
        await this.entityValidator.ValidateExistParamAsync<User>(u => u.Login == command.Login, String.Format(ErrorMessages.UserWithParamExist, DefaultConst.Login), cancellationToken);
        
        User user = this.mapper.Map<CreateUserByAdminCommand, User>(command);
        user.TariffId = InitConst.FreeTariffId;
        user.CurrencyId = InitConst.CurrencyUsdId;

        user.UserBalance = new UserBalance
        {
            Id = Guid.NewGuid(),
            CurrencyId = user.CurrencyId,
            UserId = user.Id,
            Created = DateTime.UtcNow,
            Amount = 0
        };
      
        var res = await this.userManager.CreateAsync(user, command.Password);

        if (res.Succeeded)
        {
            res = await this.userManager.AddToRoleAsync(user, RoleProvider.User);
        }
        else
        {
            if (res.Errors.Any())
            {
                string creationError = string.Join(Environment.NewLine, res.Errors.Select(e => e.Description));
                throw new LoggerException(creationError, 409);
            }
        }

        List<IdentityUserLogin<Guid>> logins = user.CreateUserLogins();

        await this.authRepository.UpdateUserLoginsAsync(user.Id, logins, cancellationToken);

        this.entityValidator.ValidateExist<User, Guid?>(user, user?.Id);

        return new MappedHelperResponse<UserResponse, User>(this.mapper.Map<User, UserResponse>(user), user);
    }
    
}