using StudentManagementSystem.Domain.Enums;

namespace StudentManagementSystem.Application.DTOs.Inquiries;

public class InquiryDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public InquiryPriority Priority { get; set; }
    public InquiryStatus Status { get; set; }
    public string InquirerName { get; set; } = string.Empty;
    public string InquirerEmail { get; set; } = string.Empty;
    public string InquirerPhone { get; set; } = string.Empty;
    public string? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }
    public DateTime? AssignedDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public string? Resolution { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateInquiryDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public InquiryPriority Priority { get; set; }
    public string InquirerName { get; set; } = string.Empty;
    public string InquirerEmail { get; set; } = string.Empty;
    public string InquirerPhone { get; set; } = string.Empty;
}

public class UpdateInquiryDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public InquiryPriority Priority { get; set; }
    public string InquirerName { get; set; } = string.Empty;
    public string InquirerEmail { get; set; } = string.Empty;
    public string InquirerPhone { get; set; } = string.Empty;
}

public class AssignInquiryDto
{
    public string InquiryId { get; set; } = string.Empty;
    public string AssignedToId { get; set; } = string.Empty;
}

public class ResolveInquiryDto
{
    public string InquiryId { get; set; } = string.Empty;
    public string Resolution { get; set; } = string.Empty;
}

public class InquirySearchDto
{
    public string? Title { get; set; }
    public string? Category { get; set; }
    public InquiryPriority? Priority { get; set; }
    public InquiryStatus? Status { get; set; }
    public string? AssignedToId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class InquirySummaryDto
{
    public int TotalInquiries { get; set; }
    public int PendingInquiries { get; set; }
    public int InProgressInquiries { get; set; }
    public int ResolvedInquiries { get; set; }
    public int OverdueInquiries { get; set; }
    public Dictionary<string, int> InquiriesByCategory { get; set; } = new();
    public Dictionary<InquiryPriority, int> InquiriesByPriority { get; set; } = new();
}

public class DashboardAnalyticsDto
{
    public DateTime Date { get; set; }
    public int TotalStudents { get; set; }
    public int NewAdmissions { get; set; }
    public decimal FeesCollected { get; set; }
    public int PendingInquiries { get; set; }
    public Dictionary<string, object> AdditionalMetrics { get; set; } = new();
}