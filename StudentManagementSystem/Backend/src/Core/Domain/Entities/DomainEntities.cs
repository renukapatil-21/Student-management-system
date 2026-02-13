using MongoDB.Bson.Serialization.Attributes;
using StudentManagementSystem.Domain.Common;
using StudentManagementSystem.Domain.Enums;

namespace StudentManagementSystem.Domain.Entities;

[AttributeUsage(AttributeTargets.Class)]
public class BsonCollectionAttribute : Attribute
{
    public string CollectionName { get; }

    public BsonCollectionAttribute(string collectionName)
    {
        CollectionName = collectionName;
    }
}

[BsonCollection("users")]
public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
    public string? ProfilePicture { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}

[BsonCollection("students")]
public class Student : BaseEntity
{
    public string StudentId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    
    // Academic Information
    public string Course { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public AcademicYear AcademicYear { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public StudentStatus Status { get; set; } = StudentStatus.Active;
    
    // Guardian Information
    public string GuardianName { get; set; } = string.Empty;
    public string GuardianPhone { get; set; } = string.Empty;
    public string GuardianEmail { get; set; } = string.Empty;
    public string GuardianRelation { get; set; } = string.Empty;
    
    // Documents
    public List<string> DocumentUrls { get; set; } = new();
    public string? ProfilePicture { get; set; }
    
    public string FullName => $"{FirstName} {LastName}";
    public int Age => DateTime.UtcNow.Year - DateOfBirth.Year;
}

[BsonCollection("fees")]
public class Fee : BaseEntity
{
    public string StudentId { get; set; } = string.Empty;
    public string FeeType { get; set; } = string.Empty; // Tuition, Lab, Library, etc.
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public FeeStatus Status { get; set; } = FeeStatus.Pending;
    public string AcademicYear { get; set; } = string.Empty;
    public string Semester { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Payment Information
    public decimal PaidAmount { get; set; } = 0;
    public DateTime? PaidDate { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public string? TransactionId { get; set; }
    public string? PaymentNotes { get; set; }
    
    public decimal RemainingAmount => Amount - PaidAmount;
    public bool IsOverdue => DueDate < DateTime.UtcNow && Status != FeeStatus.Paid;
}

[BsonCollection("inquiries")]
public class Inquiry : BaseEntity
{
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public InquiryStatus Status { get; set; } = InquiryStatus.New;
    public InquiryPriority Priority { get; set; } = InquiryPriority.Medium;
    
    // Contact Information
    public string ContactName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    
    // Assignment
    public string? AssignedToUserId { get; set; }
    public DateTime? AssignedDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    
    // Additional Information
    public string Category { get; set; } = string.Empty; // Academic, Technical, Administrative, etc.
    public List<string> Tags { get; set; } = new();
    public string? Resolution { get; set; }
    public List<InquiryComment> Comments { get; set; } = new();
    
    public bool IsOverdue => CreatedAt.AddDays(7) < DateTime.UtcNow && Status != InquiryStatus.Resolved;
}

public class InquiryComment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Comment { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsInternal { get; set; } = false; // Internal comments not visible to inquiry creator
}

[BsonCollection("dashboard_analytics")]
public class DashboardAnalytics : BaseEntity
{
    public DateTime Date { get; set; }
    public int TotalStudents { get; set; }
    public int NewEnrollments { get; set; }
    public int ActiveInquiries { get; set; }
    public int ResolvedInquiries { get; set; }
    public decimal TotalFeesCollected { get; set; }
    public decimal PendingFees { get; set; }
    public Dictionary<string, int> StudentsByDepartment { get; set; } = new();
    public Dictionary<string, decimal> FeesByType { get; set; } = new();
}