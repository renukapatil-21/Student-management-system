using MongoDB.Driver;
using MongoDB.Bson;
using StudentManagementSystem.Domain.Entities;
using StudentManagementSystem.Domain.Interfaces;
using StudentManagementSystem.Domain.Enums;
using StudentManagementSystem.Application.Interfaces.Repositories;

namespace StudentManagementSystem.Infrastructure.Persistence.Repositories;

public class InquiryRepository : GenericRepository<Inquiry>, IInquiryRepository
{
    public InquiryRepository(IMongoDatabase database) : base(database) { }

    public async Task<IEnumerable<Inquiry>> GetByStatusAsync(InquiryStatus status)
    {
        var filter = Builders<Inquiry>.Filter.Eq(x => x.Status, status);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Inquiry>> GetByPriorityAsync(InquiryPriority priority)
    {
        var filter = Builders<Inquiry>.Filter.Eq(x => x.Priority, priority);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Inquiry>> GetAssignedToUserAsync(string userId)
    {
        var filter = Builders<Inquiry>.Filter.Eq(x => x.AssignedToId, userId);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Inquiry>> GetByCategoryAsync(string category)
    {
        var filter = Builders<Inquiry>.Filter.Eq(x => x.Category, category);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Inquiry>> GetOverdueInquiriesAsync()
    {
        var threeDaysAgo = DateTime.Now.AddDays(-3);
        var filter = Builders<Inquiry>.Filter.And(
            Builders<Inquiry>.Filter.Ne(x => x.Status, InquiryStatus.Resolved),
            Builders<Inquiry>.Filter.Lt(x => x.CreatedAt, threeDaysAgo)
        );
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<Dictionary<InquiryStatus, int>> GetInquiryCountByStatusAsync()
    {
        var pipeline = new[]
        {
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", "$Status" },
                { "count", new BsonDocument("$sum", 1) }
            })
        };

        var result = await _collection.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return result.ToDictionary(
            doc => (InquiryStatus)doc["_id"].AsInt32,
            doc => doc["count"].AsInt32
        );
    }

    public async Task<Dictionary<string, int>> GetInquiryCountByCategoryAsync()
    {
        var pipeline = new[]
        {
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", "$Category" },
                { "count", new BsonDocument("$sum", 1) }
            })
        };

        var result = await _collection.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return result.ToDictionary(
            doc => doc["_id"].AsString,
            doc => doc["count"].AsInt32
        );
    }
}
}