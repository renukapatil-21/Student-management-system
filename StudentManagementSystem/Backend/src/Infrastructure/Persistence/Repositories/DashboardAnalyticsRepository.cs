using MongoDB.Driver;
using MongoDB.Bson;
using StudentManagementSystem.Domain.Entities;
using StudentManagementSystem.Domain.Interfaces;
using StudentManagementSystem.Domain.Enums;
using StudentManagementSystem.Application.Interfaces.Repositories;

namespace StudentManagementSystem.Infrastructure.Persistence.Repositories;

public class DashboardAnalyticsRepository : IDashboardAnalyticsRepository
{
    private readonly IMongoCollection<DashboardAnalytics> _collection;

    public DashboardAnalyticsRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<DashboardAnalytics>("DashboardAnalytics");
    }

    public async Task<DashboardAnalytics> GetDashboardAnalyticsAsync()
    {
        // Get total counts
        var totalStudents = await _collection.Database.GetCollection<User>("Users")
            .CountDocumentsAsync(u => u.Role == UserRole.Student);

        var totalInquiries = await _collection.Database.GetCollection<Inquiry>("Inquiries")
            .CountDocumentsAsync(Builders<Inquiry>.Filter.Empty);

        var totalFees = await _collection.Database.GetCollection<Fee>("Fees")
            .Aggregate()
            .Group(new BsonDocument
            {
                { "_id", BsonNull.Value },
                { "totalAmount", new BsonDocument("$sum", "$Amount") },
                { "paidAmount", new BsonDocument("$sum", 
                    new BsonDocument("$cond", new BsonArray 
                    { 
                        new BsonDocument("$eq", new BsonArray { "$Status", "Paid" }), 
                        "$Amount", 
                        0 
                    })) }
            })
            .FirstOrDefaultAsync();

        var activeStudents = await _collection.Database.GetCollection<Student>("Students")
            .CountDocumentsAsync(s => s.Status == StudentStatus.Active);

        return new DashboardAnalytics
        {
            Id = ObjectId.GenerateNewId().ToString(),
            TotalStudents = (int)totalStudents,
            ActiveStudents = (int)activeStudents,
            TotalInquiries = (int)totalInquiries,
            PendingInquiries = await _collection.Database.GetCollection<Inquiry>("Inquiries")
                .CountDocumentsAsync(i => i.Status == InquiryStatus.Pending),
            TotalFeesCollected = totalFees?["paidAmount"]?.AsDecimal ?? 0,
            PendingFees = totalFees != null ? totalFees["totalAmount"].AsDecimal - totalFees["paidAmount"].AsDecimal : 0,
            CreatedAt = DateTime.UtcNow
        };
    }
}