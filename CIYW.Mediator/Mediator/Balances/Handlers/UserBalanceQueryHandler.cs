using CIYW.Domain.Models.Users;
using CIYW.Interfaces;
using CIYW.Mediator.Mediator.Balances.Requests;
using MediatR;

namespace CIYW.Mediator.Mediator.Balances.Handlers;

public class UserBalanceQueryHandler: IRequestHandler<UserBalanceQuery, decimal>
{
    private readonly IReadGenericRepository<UserBalance> userBalanceRepository;
    private readonly ICurrentUserProvider currentUserProvider;

    public UserBalanceQueryHandler(
        IReadGenericRepository<UserBalance> userBalanceRepository, 
        ICurrentUserProvider currentUserProvider)
    {
        this.userBalanceRepository = userBalanceRepository;
        this.currentUserProvider = currentUserProvider;
    }

    public async Task<decimal> Handle(UserBalanceQuery request, CancellationToken cancellationToken)
    {
        Guid userId = await this.currentUserProvider.GetUserIdAsync(cancellationToken);

        decimal response = (await this.userBalanceRepository.GetByPropertyAsync(u => u.UserId == userId, cancellationToken)).Amount;

        return response;
    }
}