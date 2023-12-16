using CIYW.Domain;
using CIYW.Interfaces;
using CIYW.Models.Responses.Dictionary;
using Microsoft.EntityFrameworkCore;

namespace CIYW.Repositories;

public class DictionaryRepository: IDictionaryRepository
{
    private readonly DataContext context;

    public DictionaryRepository(DataContext context)
    {
        this.context = context;
    }

    public async Task<IList<T>> GetAllAsync<T>(CancellationToken cancellationToken) where T: class
    {
        return await this.context.Set<T>().ToListAsync(cancellationToken);
    }
}