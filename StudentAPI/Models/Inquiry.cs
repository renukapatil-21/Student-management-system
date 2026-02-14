using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StudentAPI.Models;

public class Inquiry
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("phone")]
    public string Phone { get; set; } = string.Empty;

    [BsonElement("subject")]
    public string Subject { get; set; } = string.Empty;

    [BsonElement("message")]
    public string Message { get; set; } = string.Empty;

    [BsonElement("inquiryType")]
    public InquiryType InquiryType { get; set; } = InquiryType.General;

    [BsonElement("status")]
    public InquiryStatus Status { get; set; } = InquiryStatus.New;

    [BsonElement("priority")]
    public InquiryPriority Priority { get; set; } = InquiryPriority.Medium;

    [BsonElement("assignedTo")]
    public string AssignedTo { get; set; } = string.Empty;

    [BsonElement("response")]
    public string Response { get; set; } = string.Empty;

    [BsonElement("responseDate")]
    public DateTime? ResponseDate { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum InquiryType
{
    General,
    Admission,
    Fees,
    Technical,
    Academic,
    Complaint
}

public enum InquiryStatus
{
    New,
    InProgress,
    Resolved,
    Closed,
    Cancelled
}

public enum InquiryPriority
{
    Low,
    Medium,
    High,
    Urgent
}