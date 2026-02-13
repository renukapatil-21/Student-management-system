using StudentManagementSystem.Domain.Entities;
using StudentManagementSystem.Domain.Enums;
using System.Linq.Expressions;

namespace StudentManagementSystem.Application.Interfaces.Repositories;

public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(string id);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<long> CountAsync(Expression<Func<T, bool>>? predicate = null);
    Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<T, bool>>? predicate = null);
}

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);
}

public interface IStudentRepository : IGenericRepository<Student>
{
    Task<IEnumerable<Student>> GetByStatusAsync(StudentStatus status);
    Task<IEnumerable<Student>> GetByClassAsync(string className);
    Task<IEnumerable<Student>> GetByAcademicYearAsync(AcademicYear academicYear);
    Task<IEnumerable<Student>> GetActiveStudentsAsync();
    Task<int> GetStudentCountByStatusAsync(StudentStatus status);
    Task<IEnumerable<Student>> SearchStudentsAsync(string searchTerm);
}

public interface IFeeRepository : IGenericRepository<Fee>
{
    Task<IEnumerable<Fee>> GetFeesByStudentAsync(string studentId);
    Task<IEnumerable<Fee>> GetFeesByStatusAsync(FeeStatus status);
    Task<IEnumerable<Fee>> GetOverdueFeesAsync();
    Task<decimal> GetTotalFeesCollectedAsync();
}

public interface IInquiryRepository : IGenericRepository<Inquiry>
{
    Task<IEnumerable<Inquiry>> GetInquiriesByStatusAsync(InquiryStatus status);
    Task<IEnumerable<Inquiry>> GetInquiriesByPriorityAsync(InquiryPriority priority);
    Task<IEnumerable<Inquiry>> GetRecentInquiriesAsync(int count);
}

public interface IDashboardAnalyticsRepository
{
    Task<DashboardAnalytics> GetDashboardAnalyticsAsync();
}

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IStudentRepository Students { get; }
    IFeeRepository Fees { get; }
    IInquiryRepository Inquiries { get; }
    IDashboardAnalyticsRepository DashboardAnalytics { get; }
    Task<bool> SaveChangesAsync();
}