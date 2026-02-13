using MongoDB.Driver;
using StudentManagementSystem.Domain.Interfaces;
using StudentManagementSystem.Domain.Common;
using StudentManagementSystem.Domain.Entities;
using StudentManagementSystem.Application.Interfaces.Repositories;
using System.Linq.Expressions;
using MongoDB.Bson;

namespace StudentManagementSystem.Infrastructure.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly IMongoCollection<T> _collection;

    public GenericRepository(IMongoDatabase database)
    {
        var collectionName = GetCollectionName();
        _collection = database.GetCollection<T>(collectionName);
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        var objectId = ObjectId.Parse(id);
        var filter = Builders<T>.Filter.Eq(x => x.Id, objectId);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>>? filter = null,
        int pageNumber = 1,
        int pageSize = 10,
        string sortBy = "CreatedAt",
        bool sortDescending = true)
    {
        var query = _collection.Find(filter ?? (_ => true));
        
        var sortDefinition = sortDescending 
            ? Builders<T>.Sort.Descending(sortBy)
            : Builders<T>.Sort.Ascending(sortBy);
        
        return await query
            .Sort(sortDefinition)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? filter = null)
    {
        return (int)await _collection.CountDocumentsAsync(filter ?? (_ => true));
    }

    public async Task CreateAsync(T entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        await _collection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        var filter = Builders<T>.Filter.Eq(x => x.Id, entity.Id);
        await _collection.ReplaceOneAsync(filter, entity);
    }

    public async Task DeleteAsync(string id)
    {
        var objectId = ObjectId.Parse(id);
        var filter = Builders<T>.Filter.Eq(x => x.Id, objectId);
        await _collection.DeleteOneAsync(filter);
    }

    public async Task<bool> ExistsAsync(string id)
    {
        var objectId = ObjectId.Parse(id);
        var filter = Builders<T>.Filter.Eq(x => x.Id, objectId);
        return await _collection.Find(filter).AnyAsync();
    }

    private string GetCollectionName()
    {
        var attribute = (BsonCollectionAttribute?)Attribute.GetCustomAttribute(typeof(T), typeof(BsonCollectionAttribute));
        return attribute?.CollectionName ?? typeof(T).Name.ToLowerInvariant();
    }
}