using StudentManagementSystem.Domain.Interfaces;
using StudentManagementSystem.Domain.Entities;
using StudentManagementSystem.Application.Interfaces.Repositories;
using StudentManagementSystem.Infrastructure.Persistence.Repositories;
using MongoDB.Driver;

namespace StudentManagementSystem.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly IMongoDatabase _database;
    private IUserRepository? _users;
    private IStudentRepository? _students;
    private IFeeRepository? _fees;
    private IInquiryRepository? _inquiries;
    private IDashboardAnalyticsRepository? _dashboardAnalytics;

    public UnitOfWork(IMongoDatabase database)
    {
        _database = database;
    }

    public IUserRepository Users => _users ??= new UserRepository(_database);
    public IStudentRepository Students => _students ??= new StudentRepository(_database);
    public IFeeRepository Fees => _fees ??= new FeeRepository(_database);
    public IInquiryRepository Inquiries => _inquiries ??= new InquiryRepository(_database);
    public IDashboardAnalyticsRepository DashboardAnalytics => _dashboardAnalytics ??= new DashboardAnalyticsRepository(_database);

    public async Task<bool> SaveChangesAsync()
    {
        // MongoDB doesn't require explicit save operations like EF Core
        // This method is kept for interface compatibility and future transaction support
        return await Task.FromResult(true);
    }

    public void Dispose()
    {
        // MongoDB driver handles connection disposal automatically
    }
}