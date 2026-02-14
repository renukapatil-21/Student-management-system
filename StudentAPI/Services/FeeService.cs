using MongoDB.Driver;
using StudentAPI.Models;
using Microsoft.Extensions.Options;

namespace StudentAPI.Services;

public class FeeService
{
    private readonly IMongoCollection<Fee> _feesCollection;

    public FeeService(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _feesCollection = mongoDatabase.GetCollection<Fee>(mongoDbSettings.Value.FeesCollectionName);
    }

    public async Task<List<Fee>> GetAsync() =>
        await _feesCollection.Find(_ => true).ToListAsync();

    public async Task<Fee?> GetAsync(string id) =>
        await _feesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<List<Fee>> GetByStudentIdAsync(string studentId) =>
        await _feesCollection.Find(x => x.StudentId == studentId).ToListAsync();

    public async Task<List<Fee>> GetPendingFeesAsync() =>
        await _feesCollection.Find(x => x.Status == FeeStatus.Pending || x.Status == FeeStatus.Overdue).ToListAsync();

    public async Task<List<Fee>> GetOverdueFeesAsync() =>
        await _feesCollection.Find(x => x.Status == FeeStatus.Overdue || 
            (x.Status == FeeStatus.Pending && x.DueDate < DateTime.UtcNow)).ToListAsync();

    public async Task CreateAsync(Fee newFee)
    {
        newFee.CreatedAt = DateTime.UtcNow;
        newFee.UpdatedAt = DateTime.UtcNow;
        await _feesCollection.InsertOneAsync(newFee);
    }

    public async Task UpdateAsync(string id, Fee updatedFee)
    {
        updatedFee.UpdatedAt = DateTime.UtcNow;
        await _feesCollection.ReplaceOneAsync(x => x.Id == id, updatedFee);
    }

    public async Task ProcessPaymentAsync(string feeId, decimal paymentAmount, string paymentMethod, string transactionId)
    {
        var fee = await GetAsync(feeId);
        if (fee != null)
        {
            fee.PaidAmount += paymentAmount;
            fee.PaymentDate = DateTime.UtcNow;
            fee.PaymentMethod = paymentMethod;
            fee.TransactionId = transactionId;
            fee.Status = fee.PaidAmount >= fee.Amount ? FeeStatus.Paid : FeeStatus.PartiallyPaid;
            fee.UpdatedAt = DateTime.UtcNow;

            await _feesCollection.ReplaceOneAsync(x => x.Id == feeId, fee);
        }
    }

    public async Task RemoveAsync(string id) =>
        await _feesCollection.DeleteOneAsync(x => x.Id == id);

    public async Task<decimal> GetTotalCollectedAsync() =>
        await _feesCollection.Aggregate()
            .Match(f => f.Status == FeeStatus.Paid || f.Status == FeeStatus.PartiallyPaid)
            .Group(new MongoDB.Bson.BsonDocument
            {
                { "_id", MongoDB.Bson.BsonNull.Value },
                { "total", new MongoDB.Bson.BsonDocument("$sum", "$paidAmount") }
            })
            .Project<decimal>("total")
            .FirstOrDefaultAsync();

    public async Task<long> GetPendingCountAsync() =>
        await _feesCollection.CountDocumentsAsync(f => f.Status == FeeStatus.Pending || f.Status == FeeStatus.Overdue);
}