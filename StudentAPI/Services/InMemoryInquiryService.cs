using StudentAPI.Models;
using System.Collections.Concurrent;

namespace StudentAPI.Services;

public class InMemoryInquiryService
{
    private readonly ConcurrentDictionary<string, Inquiry> _inquiries = new();

    public InMemoryInquiryService()
    {
        // Add some sample data for development
        SeedData();
    }

    private void SeedData()
    {
        var sampleInquiries = new[]
        {
            new Inquiry
            {
                Id = "inquiry1",
                Name = "Sarah Williams",
                Email = "sarah@example.com",
                Phone = "+1-555-0101",
                Subject = "Admission Process",
                Message = "I would like to know more about the admission requirements for Computer Science program.",
                InquiryType = InquiryType.Admission,
                Status = InquiryStatus.New,
                Priority = InquiryPriority.Medium,
                CreatedAt = DateTime.Now.AddHours(-2),
                UpdatedAt = DateTime.Now.AddHours(-2)
            },
            new Inquiry
            {
                Id = "inquiry2",
                Name = "Robert Brown",
                Email = "robert@example.com",
                Phone = "+1-555-0102",
                Subject = "Fee Payment Issue",
                Message = "My payment was processed but not reflected in the system. Transaction ID: TXN12345",
                InquiryType = InquiryType.Fees,
                Status = InquiryStatus.InProgress,
                Priority = InquiryPriority.High,
                AssignedTo = "Admin",
                Response = "We are investigating this issue. Please provide your student ID.",
                CreatedAt = DateTime.Now.AddDays(-1),
                UpdatedAt = DateTime.Now.AddHours(-4)
            },
            new Inquiry
            {
                Id = "inquiry3",
                Name = "Emily Davis",
                Email = "emily@example.com",
                Phone = "+1-555-0103",
                Subject = "Course Registration",
                Message = "I need help with course registration for the spring semester.",
                InquiryType = InquiryType.Academic,
                Status = InquiryStatus.Resolved,
                Priority = InquiryPriority.Medium,
                AssignedTo = "Academic Advisor",
                Response = "Course registration portal is now open. Please check your student account.",
                ResponseDate = DateTime.Now.AddHours(-12),
                CreatedAt = DateTime.Now.AddDays(-3),
                UpdatedAt = DateTime.Now.AddHours(-12)
            },
            new Inquiry
            {
                Id = "inquiry4",
                Name = "Alex Thompson",
                Email = "alex@example.com",
                Phone = "+1-555-0104",
                Subject = "Scholarship Information",
                Message = "Are there any merit-based scholarships available for international students?",
                InquiryType = InquiryType.General,
                Status = InquiryStatus.New,
                Priority = InquiryPriority.Low,
                CreatedAt = DateTime.Now.AddMinutes(-30),
                UpdatedAt = DateTime.Now.AddMinutes(-30)
            }
        };

        foreach (var inquiry in sampleInquiries)
        {
            _inquiries.TryAdd(inquiry.Id!, inquiry);
        }
    }

    public async Task<List<Inquiry>> GetAsync()
    {
        await Task.CompletedTask; // Simulate async operation
        return _inquiries.Values.OrderByDescending(i => i.CreatedAt).ToList();
    }

    public async Task<Inquiry?> GetAsync(string id)
    {
        await Task.CompletedTask;
        _inquiries.TryGetValue(id, out var inquiry);
        return inquiry;
    }

    public async Task<List<Inquiry>> GetByStatusAsync(InquiryStatus status)
    {
        await Task.CompletedTask;
        return _inquiries.Values.Where(i => i.Status == status).ToList();
    }

    public async Task<List<Inquiry>> GetByPriorityAsync(InquiryPriority priority)
    {
        await Task.CompletedTask;
        return _inquiries.Values.Where(i => i.Priority == priority).ToList();
    }

    public async Task<List<Inquiry>> GetByTypeAsync(InquiryType type)
    {
        await Task.CompletedTask;
        return _inquiries.Values.Where(i => i.InquiryType == type).ToList();
    }

    public async Task<List<Inquiry>> GetAssignedToAsync(string assignedTo)
    {
        await Task.CompletedTask;
        return _inquiries.Values.Where(i => i.AssignedTo == assignedTo).ToList();
    }

    public async Task CreateAsync(Inquiry newInquiry)
    {
        await Task.CompletedTask;
        newInquiry.Id = Guid.NewGuid().ToString();
        newInquiry.CreatedAt = DateTime.UtcNow;
        newInquiry.UpdatedAt = DateTime.UtcNow;
        _inquiries.TryAdd(newInquiry.Id, newInquiry);
    }

    public async Task UpdateAsync(string id, Inquiry updatedInquiry)
    {
        await Task.CompletedTask;
        if (_inquiries.ContainsKey(id))
        {
            updatedInquiry.UpdatedAt = DateTime.UtcNow;
            _inquiries[id] = updatedInquiry;
        }
    }

    public async Task AssignAsync(string inquiryId, string assignedTo)
    {
        await Task.CompletedTask;
        if (_inquiries.TryGetValue(inquiryId, out var inquiry))
        {
            inquiry.AssignedTo = assignedTo;
            inquiry.Status = InquiryStatus.InProgress;
            inquiry.UpdatedAt = DateTime.UtcNow;
        }
    }

    public async Task RespondAsync(string inquiryId, string response)
    {
        await Task.CompletedTask;
        if (_inquiries.TryGetValue(inquiryId, out var inquiry))
        {
            inquiry.Response = response;
            inquiry.ResponseDate = DateTime.UtcNow;
            inquiry.Status = InquiryStatus.Resolved;
            inquiry.UpdatedAt = DateTime.UtcNow;
        }
    }

    public async Task CloseAsync(string inquiryId)
    {
        await Task.CompletedTask;
        if (_inquiries.TryGetValue(inquiryId, out var inquiry))
        {
            inquiry.Status = InquiryStatus.Closed;
            inquiry.UpdatedAt = DateTime.UtcNow;
        }
    }

    public async Task RemoveAsync(string id)
    {
        await Task.CompletedTask;
        _inquiries.TryRemove(id, out _);
    }
}