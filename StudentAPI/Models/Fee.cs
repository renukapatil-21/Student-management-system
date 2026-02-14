using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StudentAPI.Models;

public class Fee
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("studentId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string StudentId { get; set; } = string.Empty;

    [BsonElement("studentName")]
    public string StudentName { get; set; } = string.Empty;

    [BsonElement("feeType")]
    public string FeeType { get; set; } = string.Empty; // Tuition, Library, Lab, etc.

    [BsonElement("amount")]
    public decimal Amount { get; set; }

    [BsonElement("dueDate")]
    public DateTime DueDate { get; set; }

    [BsonElement("paidAmount")]
    public decimal PaidAmount { get; set; } = 0;

    [BsonElement("paymentDate")]
    public DateTime? PaymentDate { get; set; }

    [BsonElement("paymentMethod")]
    public string PaymentMethod { get; set; } = string.Empty; // Cash, Card, Transfer

    [BsonElement("transactionId")]
    public string TransactionId { get; set; } = string.Empty;

    [BsonElement("status")]
    public FeeStatus Status { get; set; } = FeeStatus.Pending;

    [BsonElement("academicYear")]
    public string AcademicYear { get; set; } = string.Empty;

    [BsonElement("semester")]
    public string Semester { get; set; } = string.Empty;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum FeeStatus
{
    Pending,
    Paid,
    Overdue,
    PartiallyPaid,
    Cancelled
}