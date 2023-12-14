using System.Linq.Expressions;
using CIYW.Domain;
using CIYW.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CIYW.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly DataContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(DataContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<T>();
    }

    public async Task<IList<T>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await this._dbSet.ToListAsync(cancellationToken);
    }

    public async Task<IList<T>> GetWithPaginationAsync(int page, int take, CancellationToken cancellationToken)
    {
        return await this._dbSet.Skip((page - 1) * take).Take(take).ToListAsync(cancellationToken);
    }

    public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync(id, cancellationToken);
    }
    
    public async Task<T> GetByPropertyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }
    
    public async Task<IList<T>> GetListByPropertyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken)
    {
        await this._dbSet.AddAsync(entity, cancellationToken);
        await this._context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken)
    {
        this._context.Entry(entity).State = EntityState.Modified;
        await this._context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await this.GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            this._dbSet.Remove(entity);
            await this._context.SaveChangesAsync(cancellationToken);
        }
    }
}