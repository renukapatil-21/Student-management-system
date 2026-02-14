using Microsoft.AspNetCore.Mvc;
using StudentAPI.Models;
using StudentAPI.Services;

namespace StudentAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeesController : ControllerBase
{
    private readonly ILogger<FeesController> _logger;
    private readonly FeeService _feeService;

    public FeesController(ILogger<FeesController> logger, FeeService feeService)
    {
        _logger = logger;
        _feeService = feeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetFees()
    {
        try
        {
            var fees = await _feeService.GetAsync();
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
            var fee = await _feeService.GetAsync(id);
            
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

    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetFeesByStudent(string studentId)
    {
        try
        {
            var fees = await _feeService.GetByStudentIdAsync(studentId);
            return Ok(new { success = true, data = fees, message = "Student fees retrieved successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving fees for student: {StudentId}", studentId);
            return StatusCode(500, new { success = false, message = "Error retrieving student fees", error = ex.Message });
        }
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingFees()
    {
        try
        {
            var fees = await _feeService.GetPendingFeesAsync();
            return Ok(new { success = true, data = fees, message = "Pending fees retrieved successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending fees");
            return StatusCode(500, new { success = false, message = "Error retrieving pending fees", error = ex.Message });
        }
    }

    [HttpGet("overdue")]
    public async Task<IActionResult> GetOverdueFees()
    {
        try
        {
            var fees = await _feeService.GetOverdueFeesAsync();
            return Ok(new { success = true, data = fees, message = "Overdue fees retrieved successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving overdue fees");
            return StatusCode(500, new { success = false, message = "Error retrieving overdue fees", error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateFee([FromBody] Fee fee)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid fee data", errors = ModelState });
            }

            await _feeService.CreateAsync(fee);
            return CreatedAtAction(nameof(GetFee), new { id = fee.Id }, 
                new { success = true, data = fee, message = "Fee created successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating fee");
            return StatusCode(500, new { success = false, message = "Error creating fee", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFee(string id, [FromBody] Fee updatedFee)
    {
        try
        {
            var fee = await _feeService.GetAsync(id);

            if (fee is null)
            {
                return NotFound(new { success = false, message = "Fee not found" });
            }

            updatedFee.Id = fee.Id;
            updatedFee.CreatedAt = fee.CreatedAt;

            await _feeService.UpdateAsync(id, updatedFee);
            return Ok(new { success = true, data = updatedFee, message = "Fee updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating fee with ID: {FeeId}", id);
            return StatusCode(500, new { success = false, message = "Error updating fee", error = ex.Message });
        }
    }

    [HttpPost("{id}/payment")]
    public async Task<IActionResult> ProcessPayment(string id, [FromBody] PaymentRequest payment)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid payment data", errors = ModelState });
            }

            await _feeService.ProcessPaymentAsync(id, payment.Amount, payment.PaymentMethod, payment.TransactionId);
            return Ok(new { success = true, message = "Payment processed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment for fee: {FeeId}", id);
            return StatusCode(500, new { success = false, message = "Error processing payment", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFee(string id)
    {
        try
        {
            var fee = await _feeService.GetAsync(id);

            if (fee is null)
            {
                return NotFound(new { success = false, message = "Fee not found" });
            }

            await _feeService.RemoveAsync(id);
            return Ok(new { success = true, message = "Fee deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting fee with ID: {FeeId}", id);
            return StatusCode(500, new { success = false, message = "Error deleting fee", error = ex.Message });
        }
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetFeeStats()
    {
        try
        {
            var totalCollected = await _feeService.GetTotalCollectedAsync();
            var pendingCount = await _feeService.GetPendingCountAsync();
            
            return Ok(new { 
                success = true, 
                data = new { 
                    totalCollected, 
                    pendingCount
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
}

public class PaymentRequest
{
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
}