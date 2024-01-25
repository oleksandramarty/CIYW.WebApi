using System.Linq.Expressions;
using AutoMapper;
using CIYW.Const.Const;
using CIYW.Const.Errors;
using CIYW.Domain;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Base;
using Microsoft.EntityFrameworkCore;

namespace CIYW.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly DataContext context;
    private readonly DbSet<T> dbSet;
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IFilterProvider<T> filterProvider;
    private readonly IMapper mapper;

    public GenericRepository(
        DataContext context, 
        ICurrentUserProvider currentUserProvider,
        IFilterProvider<T> filterProvider, 
        IMapper mapper)
    {
        this.currentUserProvider = currentUserProvider;
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        this.dbSet = this.context.Set<T>();
        this.filterProvider = filterProvider;
        this.mapper = mapper;
    }

    public async Task<IList<T>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await this.dbSet.ToListAsync(cancellationToken);
    }

    public async Task<IList<T>> GetWithPaginationAsync(int page, int take, CancellationToken cancellationToken)
    {
        return await this.dbSet.Skip((page - 1) * take).Take(take).ToListAsync(cancellationToken);
    }

    public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        T entity = await dbSet.FindAsync(id, cancellationToken);
        
        return entity;
    }
    
    public async Task<T> GetWithIncludeAsync(Func<T, bool> condition, CancellationToken cancellationToken, params Func<IQueryable<T>, IQueryable<T>>[] includeFuncs)
    {
        IQueryable<T> query = dbSet;

        foreach (var includeFunc in includeFuncs)
        {
            query = includeFunc(query);
        }

        List<T> entities = await query.ToListAsync(cancellationToken);

        T entity = entities.FirstOrDefault(e => condition(e));

        return entity;
    }
    
    public async Task<ListWithIncludeHelper<TResponse>> GetListWithIncludeAsync<TResponse>(
        Expression<Func<T, bool>> condition,
        BaseFilterQuery filter,
        CancellationToken cancellationToken,
        params Func<IQueryable<T>, IQueryable<T>>[] includeFuncs)
    {
        IQueryable<T> query = dbSet;

        foreach (var includeFunc in includeFuncs)
        {
            query = includeFunc(query);
        }

        var queryResult = condition != null ? query.Where(condition) : query;       
        
        int total = await query.CountAsync(cancellationToken);
        
        queryResult = this.filterProvider.Apply(queryResult, filter);

        List<T> entities = await queryResult.ToListAsync(cancellationToken);

        return new ListWithIncludeHelper<TResponse>
        {
            Entities = entities.Select(x => this.mapper.Map<T, TResponse>(x)).ToList(),
            Paginator = filter.Paginator,
            TotalCount = total
        };
    }
   
    public async Task<T> GetByPropertyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        return await dbSet.FirstOrDefaultAsync(predicate);
    }
    
    public async Task<IList<T>> GetListByPropertyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        return await dbSet.Where(predicate).ToListAsync(cancellationToken);
    }
    public IEnumerable<T> GetEnumerableByProperty(Expression<Func<T, bool>> predicate)
    {
        return dbSet.Where(predicate).AsEnumerable();
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken)
    {
        await this.dbSet.AddAsync(entity, cancellationToken);
        await this.context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken)
    {
        this.context.Entry(entity).State = EntityState.Modified;
        await this.context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await this.GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            this.dbSet.Remove(entity);
            await this.context.SaveChangesAsync(cancellationToken);
        }
    }
}