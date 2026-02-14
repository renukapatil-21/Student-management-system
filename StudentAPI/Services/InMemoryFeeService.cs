using StudentAPI.Models;
using System.Collections.Concurrent;

namespace StudentAPI.Services;

public class InMemoryFeeService
{
    private readonly ConcurrentDictionary<string, Fee> _fees = new();

    public InMemoryFeeService()
    {
        // Add some sample data for development
        SeedData();
    }

    private void SeedData()
    {
        var sampleFees = new[]
        {
            new Fee
            {
                Id = "fee1",
                StudentId = "student1", 
                StudentName = "John Doe",
                FeeType = "Tuition",
                Amount = 5000,
                PaidAmount = 5000,
                Status = FeeStatus.Paid,
                PaymentDate = DateTime.Now.AddDays(-5),
                PaymentMethod = "Card",
                TransactionId = "TXN001",
                AcademicYear = "2025-26",
                Semester = "Fall",
                DueDate = DateTime.Now.AddDays(-10),
                CreatedAt = DateTime.Now.AddDays(-20),
                UpdatedAt = DateTime.Now.AddDays(-5)
            },
            new Fee
            {
                Id = "fee2",
                StudentId = "student2", 
                StudentName = "Jane Smith",
                FeeType = "Library",
                Amount = 500,
                PaidAmount = 500,
                Status = FeeStatus.Paid,
                PaymentDate = DateTime.Now.AddDays(-3),
                PaymentMethod = "Cash",
                TransactionId = "TXN002",
                AcademicYear = "2025-26",
                Semester = "Fall",
                DueDate = DateTime.Now.AddDays(-8),
                CreatedAt = DateTime.Now.AddDays(-15),
                UpdatedAt = DateTime.Now.AddDays(-3)
            },
            new Fee
            {
                Id = "fee3",
                StudentId = "student3", 
                StudentName = "Mike Johnson",
                FeeType = "Lab",
                Amount = 1000,
                PaidAmount = 0,
                Status = FeeStatus.Pending,
                DueDate = DateTime.Now.AddDays(5),
                AcademicYear = "2025-26",
                Semester = "Fall",
                CreatedAt = DateTime.Now.AddDays(-10),
                UpdatedAt = DateTime.Now.AddDays(-10)
            }
        };

        foreach (var fee in sampleFees)
        {
            _fees.TryAdd(fee.Id!, fee);
        }
    }

    public async Task<List<Fee>> GetAsync()
    {
        await Task.CompletedTask; // Simulate async operation
        return _fees.Values.OrderByDescending(f => f.CreatedAt).ToList();
    }

    public async Task<Fee?> GetAsync(string id)
    {
        await Task.CompletedTask;
        _fees.TryGetValue(id, out var fee);
        return fee;
    }

    public async Task<List<Fee>> GetByStudentIdAsync(string studentId)
    {
        await Task.CompletedTask;
        return _fees.Values.Where(f => f.StudentId == studentId).ToList();
    }

    public async Task<List<Fee>> GetPendingFeesAsync()
    {
        await Task.CompletedTask;
        return _fees.Values.Where(f => f.Status == FeeStatus.Pending || f.Status == FeeStatus.Overdue).ToList();
    }

    public async Task<List<Fee>> GetOverdueFeesAsync()
    {
        await Task.CompletedTask;
        return _fees.Values.Where(f => f.Status == FeeStatus.Overdue || 
            (f.Status == FeeStatus.Pending && f.DueDate < DateTime.UtcNow)).ToList();
    }

    public async Task CreateAsync(Fee newFee)
    {
        await Task.CompletedTask;
        newFee.Id = Guid.NewGuid().ToString();
        newFee.CreatedAt = DateTime.UtcNow;
        newFee.UpdatedAt = DateTime.UtcNow;
        _fees.TryAdd(newFee.Id, newFee);
    }

    public async Task UpdateAsync(string id, Fee updatedFee)
    {
        await Task.CompletedTask;
        if (_fees.ContainsKey(id))
        {
            updatedFee.UpdatedAt = DateTime.UtcNow;
            _fees[id] = updatedFee;
        }
    }

    public async Task ProcessPaymentAsync(string feeId, decimal paymentAmount, string paymentMethod, string transactionId)
    {
        await Task.CompletedTask;
        if (_fees.TryGetValue(feeId, out var fee))
        {
            fee.PaidAmount += paymentAmount;
            fee.PaymentMethod = paymentMethod;
            fee.TransactionId = transactionId;
            fee.PaymentDate = DateTime.UtcNow;
            fee.UpdatedAt = DateTime.UtcNow;
            
            if (fee.PaidAmount >= fee.Amount)
            {
                fee.Status = FeeStatus.Paid;
            }
            else if (fee.PaidAmount > 0)
            {
                fee.Status = FeeStatus.PartiallyPaid;
            }
        }
    }

    public async Task RemoveAsync(string id)
    {
        await Task.CompletedTask;
        _fees.TryRemove(id, out _);
    }
}