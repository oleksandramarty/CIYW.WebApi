using CIYW.Models.Helpers.Base;

namespace CIYW.Interfaces.Strategies;

public interface IPageableStrategyRepository<T, TFilter> where T : class
{
    Task<ListWithIncludeHelper<T>> GetPageableAsync(TFilter filter, CancellationToken cancellationToken);
}