using StudentManagementSystem.Domain.Entities;
using StudentManagementSystem.Domain.Enums;

namespace StudentManagementSystem.Domain.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetByRoleAsync(UserRole role);
    Task<bool> EmailExistsAsync(string email);
}

public interface IStudentRepository : IGenericRepository<Student>
{
    Task<Student?> GetByStudentIdAsync(string studentId);
    Task<Student?> GetByEmailAsync(string email);
    Task<IEnumerable<Student>> GetByDepartmentAsync(string department);
    Task<IEnumerable<Student>> GetByStatusAsync(StudentStatus status);
    Task<IEnumerable<Student>> GetByAcademicYearAsync(AcademicYear academicYear);
    Task<bool> StudentIdExistsAsync(string studentId);
    Task<bool> EmailExistsAsync(string email);
    Task<Dictionary<string, int>> GetStudentCountByDepartmentAsync();
    Task<Dictionary<StudentStatus, int>> GetStudentCountByStatusAsync();
}

public interface IFeeRepository : IGenericRepository<Fee>
{
    Task<IEnumerable<Fee>> GetByStudentIdAsync(string studentId);
    Task<IEnumerable<Fee>> GetByStatusAsync(FeeStatus status);
    Task<IEnumerable<Fee>> GetOverdueFeesAsync();
    Task<IEnumerable<Fee>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<decimal> GetTotalFeesCollectedAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<decimal> GetPendingFeesAsync();
    Task<Dictionary<string, decimal>> GetFeesSummaryByTypeAsync();
}

public interface IInquiryRepository : IGenericRepository<Inquiry>
{
    Task<IEnumerable<Inquiry>> GetByStatusAsync(InquiryStatus status);
    Task<IEnumerable<Inquiry>> GetByPriorityAsync(InquiryPriority priority);
    Task<IEnumerable<Inquiry>> GetAssignedToUserAsync(string userId);
    Task<IEnumerable<Inquiry>> GetByCategoryAsync(string category);
    Task<IEnumerable<Inquiry>> GetOverdueInquiriesAsync();
    Task<Dictionary<InquiryStatus, int>> GetInquiryCountByStatusAsync();
    Task<Dictionary<string, int>> GetInquiryCountByCategoryAsync();
}

public interface IDashboardAnalyticsRepository : IGenericRepository<DashboardAnalytics>
{
    Task<DashboardAnalytics?> GetByDateAsync(DateTime date);
    Task<IEnumerable<DashboardAnalytics>> GetDateRangeAsync(DateTime startDate, DateTime endDate);
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