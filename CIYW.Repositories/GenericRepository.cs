﻿using System.Linq.Expressions;
using CIYW.Const.Const;
using CIYW.Const.Enum;
using CIYW.Const.Errors;
using CIYW.Domain;
using CIYW.Interfaces;
using CIYW.Kernel.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CIYW.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly DataContext context;
    private readonly DbSet<T> dbSet;
    private readonly ICurrentUserProvider currentUserProvider;

    public GenericRepository(
        DataContext context, 
        ICurrentUserProvider currentUserProvider)
    {
        this.currentUserProvider = currentUserProvider;
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        this.dbSet = this.context.Set<T>();
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

        await this.CheckEntityExistsAsync(entity, cancellationToken);
        
        return entity;
    }
    
    public async Task<T> GetWithIncludeAsync(Func<T, bool> condition, Func<IQueryable<T>, IQueryable<T>> includeFunc, CancellationToken cancellationToken)
    {
        IQueryable<T> query = dbSet;

        query = includeFunc(query);

        List<T> entities = await query.ToListAsync(cancellationToken);

        T entity = entities.FirstOrDefault(e => condition(e));

        await this.CheckEntityExistsAsync(entity, cancellationToken);

        return entity;
    }
   
    public async Task<T> GetByPropertyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        return await dbSet.FirstOrDefaultAsync(predicate);
    }
    
    public async Task<IList<T>> GetListByPropertyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        return await dbSet.Where(predicate).ToListAsync(cancellationToken);
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
        
    private async Task CheckEntityExistsAsync(T entity, CancellationToken cancellationToken)
    {
        if (entity == null)
        {
            Guid userId = await this.currentUserProvider.GetUserIdAsync(cancellationToken);
            throw new LoggerException($"{typeof(T).Name} {ErrorMessages.NotFound}", 404, userId, EntityTypeEnum.Generic.ToString());
        }
    }
}