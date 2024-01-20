using System.Security.Claims;
using CIYW.Const.Errors;
using CIYW.Const.Providers;
using CIYW.Domain;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Kernel.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CIYW.Repositories;

public class AuthRepository: IAuthRepository
{
    private readonly UserManager<User> userManager;
    private readonly DataContext context;

    public AuthRepository(UserManager<User> userManager, DataContext context)
    {
        this.userManager = userManager;
        this.context = context;
    }
    
    public async Task<string> GetAuthenticationTokenAsync(User user, string loginProvider, string tokenName)
    {
        return await this.userManager.GetAuthenticationTokenAsync(user, loginProvider, tokenName);
    }
    
    public async Task<User> FindUserByIdAsync(string userId)
    {
        return await this.userManager.FindByIdAsync(userId);
    }
    
    public async Task UpdateUserLoginsAsync(Guid userId, IList<IdentityUserLogin<Guid>> entity,
        CancellationToken cancellationToken)
    {
        using (var transaction = this.context.Database.BeginTransaction())
        {
            try
            {
                var oldLogins = await this.context.UserLogins.Where(_ => _.UserId == userId)
                    .ToListAsync(cancellationToken);
                if (oldLogins != null)
                {
                    this.context.UserLogins.RemoveRange(oldLogins);
                    await this.context.SaveChangesAsync(cancellationToken);
                }

                await this.context.UserLogins.AddRangeAsync(entity, cancellationToken);
                await this.context.SaveChangesAsync(cancellationToken);

                transaction.Commit();
                return;
            }
            catch (LoggerException e)
            {
                transaction.Rollback();
                throw;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
    
    public async Task UpdateUserLoginsAsync(User user, CancellationToken cancellationToken)
    {
        using (var transaction = this.context.Database.BeginTransaction())
        {
            try
            {
                await this.UpdateLoginAsync(user.Id, LoginProvider.CIYWLogin, user.Login, cancellationToken);
                await this.UpdateLoginAsync(user.Id, LoginProvider.CIYWPhone, user.PhoneNumber, cancellationToken);
                await this.UpdateLoginAsync(user.Id, LoginProvider.CIYWEmail, user.Email, cancellationToken);

                transaction.Commit();
                return;
            }
            catch (LoggerException e)
            {
                transaction.Rollback();
                throw;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    private async Task UpdateLoginAsync(Guid userId, string provider, string value, CancellationToken cancellationToken)
    {
        if (value.NotNullOrEmpty())
        {
            var login = await this.context.UserLogins.FirstOrDefaultAsync(
                l => l.UserId == userId && l.LoginProvider == provider, cancellationToken);
            if (login == null)
            {
                throw new LoggerException($"{provider} {ErrorMessages.NotFound}", 404, userId);
            }

            login.ProviderKey = value;
            login.ProviderDisplayName = value;
            this.context.UserLogins.Update(login);
            await this.context.SaveChangesAsync(cancellationToken);
        }
    } 
    
    public async Task LogOutUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await this.context.Users.FirstOrDefaultAsync(_ => _.Id == userId, cancellationToken);
        if (user == null)
        {
            throw new LoggerException(ErrorMessages.UserNotFound, 404, userId);
        }

        var logins = await this.context.UserLogins.Where(_ => _.UserId == user.Id).ToListAsync(cancellationToken);

        foreach (var login in logins)
        {
            await this.userManager.RemoveAuthenticationTokenAsync(user, login.LoginProvider, TokenNameProvider.CIYWAuth);
        }
    }
    
    public async Task<User> FindUserByLoginAsync(string loginProvider, string providerKey)
    {
        User user = await this.userManager.FindByLoginAsync(loginProvider, providerKey);
        return user;
    }
    
    public async Task<IList<string>> GetRolesAsync(User user)
    {
        return await this.userManager.GetRolesAsync(user);
    }
    
    public async Task<bool> CheckPasswordAsync(User user, string password)
    {
        return await this.userManager.CheckPasswordAsync(user, password);
    }
    
    public async Task<IdentityResult> SetAuthenticationTokenAsync(User user, string loginProvider,
        string tokenName, string tokenValue)
    {
        return await this.userManager.SetAuthenticationTokenAsync(user, loginProvider, tokenName,
            tokenValue);
    }
}