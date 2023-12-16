namespace CIYW.Interfaces;

public interface IDictionaryRepository
{
    Task<IList<T>> GetAllAsync<T>(CancellationToken cancellationToken) where T : class;
}