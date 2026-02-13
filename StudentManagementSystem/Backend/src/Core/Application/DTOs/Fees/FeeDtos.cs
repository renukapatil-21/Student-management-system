using StudentManagementSystem.Domain.Enums;

namespace StudentManagementSystem.Application.DTOs.Fees;

public class FeeDto
{
    public string Id { get; set; } = string.Empty;
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string FeeType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public FeeStatus Status { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public string? TransactionId { get; set; }
    public string? Remarks { get; set; }
    public AcademicYear AcademicYear { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateFeeDto
{
    public string StudentId { get; set; } = string.Empty;
    public string FeeType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public AcademicYear AcademicYear { get; set; }
    public string? Remarks { get; set; }
}

public class UpdateFeeDto
{
    public string FeeType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public string? Remarks { get; set; }
}

public class PayFeeDto
{
    public string FeeId { get; set; } = string.Empty;
    public PaymentMethod PaymentMethod { get; set; }
    public string? TransactionId { get; set; }
    public DateTime PaymentDate { get; set; }
    public string? PaymentRemarks { get; set; }
}

public class FeeSearchDto
{
    public string? StudentId { get; set; }
    public string? FeeType { get; set; }
    public FeeStatus? Status { get; set; }
    public AcademicYear? AcademicYear { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class FeesSummaryDto
{
    public decimal TotalFeesCollected { get; set; }
    public decimal PendingFees { get; set; }
    public decimal OverdueFees { get; set; }
    public int TotalStudentsWithPendingFees { get; set; }
    public Dictionary<string, decimal> FeesByType { get; set; } = new();
    public Dictionary<PaymentMethod, decimal> PaymentsByMethod { get; set; } = new();
}