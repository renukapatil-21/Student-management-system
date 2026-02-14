using MongoDB.Driver;
using StudentAPI.Models;
using Microsoft.Extensions.Options;

namespace StudentAPI.Services;

public class InquiryService
{
    private readonly IMongoCollection<Inquiry> _inquiriesCollection;

    public InquiryService(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _inquiriesCollection = mongoDatabase.GetCollection<Inquiry>(mongoDbSettings.Value.InquiriesCollectionName);
    }

    public async Task<List<Inquiry>> GetAsync() =>
        await _inquiriesCollection.Find(_ => true).SortByDescending(i => i.CreatedAt).ToListAsync();

    public async Task<Inquiry?> GetAsync(string id) =>
        await _inquiriesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<List<Inquiry>> GetByStatusAsync(InquiryStatus status) =>
        await _inquiriesCollection.Find(x => x.Status == status).SortByDescending(i => i.CreatedAt).ToListAsync();

    public async Task<List<Inquiry>> GetByTypeAsync(InquiryType type) =>
        await _inquiriesCollection.Find(x => x.InquiryType == type).SortByDescending(i => i.CreatedAt).ToListAsync();

    public async Task<List<Inquiry>> GetNewInquiriesAsync() =>
        await _inquiriesCollection.Find(x => x.Status == InquiryStatus.New).SortByDescending(i => i.CreatedAt).ToListAsync();

    public async Task<List<Inquiry>> SearchAsync(string searchTerm)
    {
        var filter = Builders<Inquiry>.Filter.Or(
            Builders<Inquiry>.Filter.Regex(i => i.Name, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
            Builders<Inquiry>.Filter.Regex(i => i.Email, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
            Builders<Inquiry>.Filter.Regex(i => i.Subject, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
            Builders<Inquiry>.Filter.Regex(i => i.Message, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
        );

        return await _inquiriesCollection.Find(filter).SortByDescending(i => i.CreatedAt).ToListAsync();
    }

    public async Task CreateAsync(Inquiry newInquiry)
    {
        newInquiry.CreatedAt = DateTime.UtcNow;
        newInquiry.UpdatedAt = DateTime.UtcNow;
        await _inquiriesCollection.InsertOneAsync(newInquiry);
    }

    public async Task UpdateAsync(string id, Inquiry updatedInquiry)
    {
        updatedInquiry.UpdatedAt = DateTime.UtcNow;
        await _inquiriesCollection.ReplaceOneAsync(x => x.Id == id, updatedInquiry);
    }

    public async Task UpdateStatusAsync(string id, InquiryStatus status)
    {
        var update = Builders<Inquiry>.Update
            .Set(i => i.Status, status)
            .Set(i => i.UpdatedAt, DateTime.UtcNow);

        await _inquiriesCollection.UpdateOneAsync(x => x.Id == id, update);
    }

    public async Task AddResponseAsync(string id, string response, string respondedBy)
    {
        var update = Builders<Inquiry>.Update
            .Set(i => i.Response, response)
            .Set(i => i.ResponseDate, DateTime.UtcNow)
            .Set(i => i.AssignedTo, respondedBy)
            .Set(i => i.Status, InquiryStatus.Resolved)
            .Set(i => i.UpdatedAt, DateTime.UtcNow);

        await _inquiriesCollection.UpdateOneAsync(x => x.Id == id, update);
    }

    public async Task RemoveAsync(string id) =>
        await _inquiriesCollection.DeleteOneAsync(x => x.Id == id);

    public async Task<long> GetTotalCountAsync() =>
        await _inquiriesCollection.CountDocumentsAsync(_ => true);

    public async Task<long> GetNewCountAsync() =>
        await _inquiriesCollection.CountDocumentsAsync(i => i.Status == InquiryStatus.New);

    public async Task<long> GetPendingCountAsync() =>
        await _inquiriesCollection.CountDocumentsAsync(i => i.Status == InquiryStatus.New || i.Status == InquiryStatus.InProgress);
}