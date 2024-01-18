using System.Linq.Expressions;
using AutoMapper;
using CIYW.Interfaces;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Requests.Common;
using CIYW.MongoDB;
using MongoDB.Driver;

namespace CIYW.Repositories;

public class MongoDbRepository<T>: IMongoDbRepository<T>
{
    private readonly IMongoCollection<T> imagesCollection;
    private readonly IFilterProvider<T> filterProvider;
    private readonly IMapper mapper;

    public MongoDbRepository(
        IMongoClient mongoClient, 
        IMongoDbSettings mongoDbSettings, 
        IFilterProvider<T> filterProvider, 
        IMapper mapper)
    {
        this.filterProvider = filterProvider;
        this.mapper = mapper;
        var databaseImages = mongoClient.GetDatabase(mongoDbSettings.DatabaseNameImages);
        this.imagesCollection = databaseImages.GetCollection<T>(mongoDbSettings.CollectionNameImages);
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await this.imagesCollection.Find(_ => true).ToListAsync(cancellationToken);
    }

    public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var filter = Builders<T>.Filter.Eq("_id", id);
        return await this.imagesCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken)
    {
        return await this.imagesCollection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task CreateAsync(T entity, CancellationToken cancellationToken)
    {
        await this.imagesCollection.InsertOneAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(Guid id, T entity)
    {
        var filter = Builders<T>.Filter.Eq("_id", id);
        await this.imagesCollection.ReplaceOneAsync(filter, entity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var filter = Builders<T>.Filter.Eq("_id", id);
        await this.imagesCollection.DeleteOneAsync(filter, cancellationToken);
    }
    
    public async Task<ListWithIncludeHelper<TResponse>> GetPageableListAsync<TResponse>(BaseFilterQuery filter, CancellationToken cancellationToken)
    {
        var query = this.imagesCollection.Find(_ => true);

        long total = await query.CountDocumentsAsync(cancellationToken);
        
        IList<T> entities = await this.filterProvider.Apply(query, filter).ToListAsync(cancellationToken);
        
        return new ListWithIncludeHelper<TResponse>
        {
            Entities = entities.Select(x => this.mapper.Map<T, TResponse>(x)).ToList(),
            Paginator = filter.Paginator,
            TotalCount = total
        };
    }
}