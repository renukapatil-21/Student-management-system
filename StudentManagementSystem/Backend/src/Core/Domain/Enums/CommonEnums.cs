namespace StudentManagementSystem.Domain.Enums;

public enum Gender
{
    Male,
    Female,
    Other
}

public enum StudentStatus
{
    Active,
    Inactive,
    Graduated,
    Suspended,
    Withdrawn
}

public enum FeeStatus
{
    Pending,
    Paid,
    Overdue,
    Partial,
    Cancelled
}

public enum InquiryStatus
{
    New,
    InProgress,
    Resolved,
    Closed,
    Escalated
}

public enum InquiryPriority
{
    Low,
    Medium,
    High,
    Critical
}

public enum UserRole
{
    Admin,
    Staff,
    Student,
    Parent
}

public enum PaymentMethod
{
    Cash,
    Card,
    BankTransfer,
    OnlinePayment,
    Cheque
}

public enum AcademicYear
{
    FirstYear = 1,
    SecondYear = 2,
    ThirdYear = 3,
    FourthYear = 4
}