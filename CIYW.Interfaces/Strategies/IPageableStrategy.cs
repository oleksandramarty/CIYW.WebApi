using CIYW.Models.Helpers.Base;

namespace CIYW.Interfaces.Strategies;

public interface IPageableStrategy<T, TFilter>
{
    Task<ListWithIncludeHelper<T>> GetPageableAsync(TFilter filter, CancellationToken cancellationToken);
}
