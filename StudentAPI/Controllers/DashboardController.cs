using Microsoft.AspNetCore.Mvc;
using StudentAPI.Models;
using StudentAPI.Services;

namespace StudentAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly ILogger<DashboardController> _logger;
    private readonly IStudentService _studentService;
    private readonly InMemoryFeeService? _inMemoryFeeService;
    private readonly InMemoryInquiryService? _inMemoryInquiryService;
    private readonly FeeService? _feeService;
    private readonly InquiryService? _inquiryService;

    public DashboardController(
        ILogger<DashboardController> logger,
        IStudentService studentService,
        InMemoryFeeService? inMemoryFeeService = null,
        InMemoryInquiryService? inMemoryInquiryService = null,
        FeeService? feeService = null,
        InquiryService? inquiryService = null)
    {
        _logger = logger;
        _studentService = studentService;
        _inMemoryFeeService = inMemoryFeeService;
        _inMemoryInquiryService = inMemoryInquiryService;
        _feeService = feeService;
        _inquiryService = inquiryService;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetDashboardStats()
    {
        try
        {
            // Get all data
            var students = await _studentService.GetAsync();
            var fees = _feeService != null ? await _feeService.GetAsync() : 
                      _inMemoryFeeService != null ? await _inMemoryFeeService.GetAsync() : 
                      new List<Fee>();
            var inquiries = _inquiryService != null ? await _inquiryService.GetAsync() : 
                           _inMemoryInquiryService != null ? await _inMemoryInquiryService.GetAsync() : 
                           new List<Inquiry>();

            // Calculate statistics
            var totalStudents = students.Count();
            
            // Count new admissions (students added in the last 30 days)
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);
            var newAdmissions = students.Count(s => s.EnrollmentDate >= thirtyDaysAgo);

            // Calculate total fees collected
            var feesCollected = fees
                .Where(f => f.Status == FeeStatus.Paid)
                .Sum(f => f.PaidAmount);

            // Count pending inquiries
            var pendingInquiries = inquiries.Count(i => 
                i.Status == InquiryStatus.New || i.Status == InquiryStatus.InProgress);

            var stats = new
            {
                totalStudents,
                newAdmissions,
                feesCollected,
                pendingInquiries,
                lastUpdated = DateTime.Now
            };

            return Ok(new 
            { 
                success = true, 
                data = stats, 
                message = "Dashboard statistics retrieved successfully" 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard statistics");
            return StatusCode(500, new 
            { 
                success = false, 
                message = "Error retrieving dashboard statistics", 
                error = ex.Message 
            });
        }
    }

    [HttpGet("recent-activities")]
    public async Task<IActionResult> GetRecentActivities()
    {
        try
        {
            var activities = new List<object>();
            
            // Get recent students (last 10)
            var students = await _studentService.GetAsync();
            var recentStudents = students
                .OrderByDescending(s => s.CreatedAt)
                .Take(5);

            foreach (var student in recentStudents)
            {
                activities.Add(new
                {
                    type = "admission",
                    message = $"New student {student.FirstName} {student.LastName} admitted to {student.Course}",
                    time = GetTimeAgo(student.CreatedAt),
                    timestamp = student.CreatedAt.ToString("O")
                });
            }

            // Get recent fees (last 5 payments)
            var fees = _feeService != null ? await _feeService.GetAsync() : 
                      _inMemoryFeeService != null ? await _inMemoryFeeService.GetAsync() : 
                      new List<Fee>();
            var recentPayments = fees
                .Where(f => f.Status == FeeStatus.Paid)
                .OrderByDescending(f => f.UpdatedAt)
                .Take(3);

            foreach (var fee in recentPayments)
            {
                activities.Add(new
                {
                    type = "payment",
                    message = $"Fee payment of ${fee.PaidAmount:F2} received from {fee.StudentName}",
                    time = GetTimeAgo(fee.UpdatedAt),
                    timestamp = fee.UpdatedAt.ToString("O")
                });
            }

            // Get recent inquiries (last 3)
            var inquiries = _inquiryService != null ? await _inquiryService.GetAsync() : 
                           _inMemoryInquiryService != null ? await _inMemoryInquiryService.GetAsync() : 
                           new List<Inquiry>();
            var recentInquiries = inquiries
                .OrderByDescending(i => i.CreatedAt)
                .Take(2);

            foreach (var inquiry in recentInquiries)
            {
                activities.Add(new
                {
                    type = "inquiry",
                    message = $"New inquiry from {inquiry.Name} about {inquiry.Subject}",
                    time = GetTimeAgo(inquiry.CreatedAt),
                    timestamp = inquiry.CreatedAt.ToString("O")
                });
            }

            // Sort all activities by timestamp (most recent first)
            var sortedActivities = activities
                .OrderByDescending(a => DateTime.TryParse(a.GetType().GetProperty("timestamp")?.GetValue(a)?.ToString(), out var ts) ? ts : DateTime.MinValue)
                .Take(10)
                .ToList();

            return Ok(new 
            { 
                success = true, 
                data = sortedActivities, 
                message = "Recent activities retrieved successfully" 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recent activities");
            return StatusCode(500, new 
            { 
                success = false, 
                message = "Error retrieving recent activities", 
                error = ex.Message 
            });
        }
    }

    private string GetTimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.Now - dateTime;
        
        if (timeSpan.TotalMinutes < 1)
            return "Just now";
        if (timeSpan.TotalMinutes < 60)
            return $"{(int)timeSpan.TotalMinutes} minutes ago";
        if (timeSpan.TotalHours < 24)
            return $"{(int)timeSpan.TotalHours} hours ago";
        if (timeSpan.TotalDays < 7)
            return $"{(int)timeSpan.TotalDays} days ago";
        if (timeSpan.TotalDays < 30)
            return $"{(int)(timeSpan.TotalDays / 7)} weeks ago";
        
        return dateTime.ToString("MMM dd, yyyy");
    }
}