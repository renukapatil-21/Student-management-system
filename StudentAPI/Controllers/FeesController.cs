using Microsoft.AspNetCore.Mvc;
using StudentAPI.Models;
using StudentAPI.Services;

namespace StudentAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeesController : ControllerBase
{
    private readonly ILogger<FeesController> _logger;
    private readonly FeeService? _feeService;
    private readonly InMemoryFeeService? _inMemoryFeeService;

    public FeesController(ILogger<FeesController> logger, FeeService? feeService = null, InMemoryFeeService? inMemoryFeeService = null)
    {
        _logger = logger;
        _feeService = feeService;
        _inMemoryFeeService = inMemoryFeeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetFees()
    {
        try
        {
            var fees = _feeService != null ? await _feeService.GetAsync() : 
                      _inMemoryFeeService != null ? await _inMemoryFeeService.GetAsync() : 
                      new List<Fee>();
            return Ok(new { success = true, data = fees, message = "Fees retrieved successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving fees");
            return StatusCode(500, new { success = false, message = "Error retrieving fees", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFee(string id)
    {
        try
        {
            var fee = _feeService != null ? await _feeService.GetAsync(id) : 
                     _inMemoryFeeService != null ? await _inMemoryFeeService.GetAsync(id) : 
                     null;
            
            if (fee is null)
            {
                return NotFound(new { success = false, message = "Fee not found" });
            }

            return Ok(new { success = true, data = fee, message = "Fee retrieved successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving fee with ID: {FeeId}", id);
            return StatusCode(500, new { success = false, message = "Error retrieving fee", error = ex.Message });
        }
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetFeeStatistics()
    {
        try
        {
            var fees = _feeService != null ? await _feeService.GetAsync() : 
                      _inMemoryFeeService != null ? await _inMemoryFeeService.GetAsync() : 
                      new List<Fee>();

            var totalCollected = fees.Where(f => f.Status == FeeStatus.Paid).Sum(f => f.Amount);
            var pendingAmount = fees.Where(f => f.Status == FeeStatus.Pending).Sum(f => f.Amount);
            var overdueAmount = fees.Where(f => f.Status == FeeStatus.Overdue).Sum(f => f.Amount);
            var totalStudents = fees.Select(f => f.StudentId).Distinct().Count();

            return Ok(new
            {
                success = true,
                data = new
                {
                    totalCollected,
                    pendingAmount,
                    overdueAmount,
                    totalStudents,
                    totalFees = fees.Count
                },
                message = "Fee statistics retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving fee statistics");
            return StatusCode(500, new { success = false, message = "Error retrieving statistics", error = ex.Message });
        }
    }

    [HttpPost("{id}/payment")]
    public async Task<IActionResult> ProcessPayment(string id, [FromBody] PaymentRequest payment)
    {
        try
        {
            var fee = _feeService != null ? await _feeService.GetAsync(id) : 
                     _inMemoryFeeService != null ? await _inMemoryFeeService.GetAsync(id) : 
                     null;
            
            if (fee is null)
            {
                return NotFound(new { success = false, message = "Fee not found" });
            }

            if (fee.Status == FeeStatus.Paid)
            {
                return BadRequest(new { success = false, message = "Fee is already paid" });
            }

            // Update fee status and payment details
            fee.Status = FeeStatus.Paid;
            fee.PaidAmount = payment.Amount;
            fee.PaymentDate = DateTime.UtcNow;
            fee.PaymentMethod = payment.PaymentMethod;
            fee.TransactionId = payment.TransactionId;
            fee.UpdatedAt = DateTime.UtcNow;

            if (_feeService != null)
                await _feeService.UpdateAsync(id, fee);
            else if (_inMemoryFeeService != null)
                await _inMemoryFeeService.UpdateAsync(id, fee);

            return Ok(new { success = true, data = fee, message = "Payment processed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment for fee ID: {FeeId}", id);
            return StatusCode(500, new { success = false, message = "Error processing payment", error = ex.Message });
        }
    }
}

public class PaymentRequest
{
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
}
