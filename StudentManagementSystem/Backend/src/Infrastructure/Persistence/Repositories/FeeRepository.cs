using MongoDB.Driver;
using MongoDB.Bson;
using StudentManagementSystem.Domain.Entities;
using StudentManagementSystem.Domain.Interfaces;
using StudentManagementSystem.Domain.Enums;
using StudentManagementSystem.Application.Interfaces.Repositories;

namespace StudentManagementSystem.Infrastructure.Persistence.Repositories;

public class FeeRepository : GenericRepository<Fee>, IFeeRepository
{
    public FeeRepository(IMongoDatabase database) : base(database) { }

    public async Task<IEnumerable<Fee>> GetByStudentIdAsync(string studentId)
    {
        var filter = Builders<Fee>.Filter.Eq(x => x.StudentId, studentId);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Fee>> GetByStatusAsync(FeeStatus status)
    {
        var filter = Builders<Fee>.Filter.Eq(x => x.Status, status);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Fee>> GetOverdueFeesAsync()
    {
        var filter = Builders<Fee>.Filter.And(
            Builders<Fee>.Filter.Eq(x => x.Status, FeeStatus.Pending),
            Builders<Fee>.Filter.Lt(x => x.DueDate, DateTime.Now)
        );
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Fee>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var filter = Builders<Fee>.Filter.And(
            Builders<Fee>.Filter.Gte(x => x.CreatedAt, startDate),
            Builders<Fee>.Filter.Lte(x => x.CreatedAt, endDate)
        );
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<decimal> GetTotalFeesCollectedAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var filterBuilder = Builders<Fee>.Filter;
        var filter = filterBuilder.Eq(x => x.Status, FeeStatus.Paid);

        if (startDate.HasValue)
            filter = filterBuilder.And(filter, filterBuilder.Gte(x => x.PaidDate, startDate.Value));
        
        if (endDate.HasValue)
            filter = filterBuilder.And(filter, filterBuilder.Lte(x => x.PaidDate, endDate.Value));

        var pipeline = new[]
        {
            new BsonDocument("$match", filter.Render(_collection.DocumentSerializer, _collection.Settings.SerializerRegistry)),
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", BsonNull.Value },
                { "total", new BsonDocument("$sum", "$Amount") }
            })
        };

        var result = await _collection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
        return result?["total"].AsDecimal ?? 0;
    }

    public async Task<decimal> GetPendingFeesAsync()
    {
        var filter = Builders<Fee>.Filter.Eq(x => x.Status, FeeStatus.Pending);
        
        var pipeline = new[]
        {
            new BsonDocument("$match", filter.Render(_collection.DocumentSerializer, _collection.Settings.SerializerRegistry)),
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", BsonNull.Value },
                { "total", new BsonDocument("$sum", "$Amount") }
            })
        };

        var result = await _collection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
        return result?["total"].AsDecimal ?? 0;
    }

    public async Task<Dictionary<string, decimal>> GetFeesSummaryByTypeAsync()
    {
        var pipeline = new[]
        {
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", "$FeeType" },
                { "total", new BsonDocument("$sum", "$Amount") }
            })
        };

        var result = await _collection.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return result.ToDictionary(
            doc => doc["_id"].AsString,
            doc => doc["total"].AsDecimal
        );
    }
}