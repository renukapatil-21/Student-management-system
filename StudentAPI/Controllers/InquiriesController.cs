using Microsoft.AspNetCore.Mvc;
using StudentAPI.Models;
using StudentAPI.Services;

namespace StudentAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InquiriesController : ControllerBase
{
    private readonly ILogger<InquiriesController> _logger;
    private readonly InquiryService _inquiryService;

    public InquiriesController(ILogger<InquiriesController> logger, InquiryService inquiryService)
    {
        _logger = logger;
        _inquiryService = inquiryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetInquiries()
    {
        try
        {
            var inquiries = await _inquiryService.GetAsync();
            return Ok(new { success = true, data = inquiries, message = "Inquiries retrieved successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inquiries");
            return StatusCode(500, new { success = false, message = "Error retrieving inquiries", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetInquiry(string id)
    {
        try
        {
            var inquiry = await _inquiryService.GetAsync(id);
            
            if (inquiry is null)
            {
                return NotFound(new { success = false, message = "Inquiry not found" });
            }

            return Ok(new { success = true, data = inquiry, message = "Inquiry retrieved successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inquiry with ID: {InquiryId}", id);
            return StatusCode(500, new { success = false, message = "Error retrieving inquiry", error = ex.Message });
        }
    }

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetInquiriesByStatus(InquiryStatus status)
    {
        try
        {
            var inquiries = await _inquiryService.GetByStatusAsync(status);
            return Ok(new { success = true, data = inquiries, message = $"Inquiries with status '{status}' retrieved successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inquiries by status: {Status}", status);
            return StatusCode(500, new { success = false, message = "Error retrieving inquiries", error = ex.Message });
        }
    }

    [HttpGet("type/{type}")]
    public async Task<IActionResult> GetInquiriesByType(InquiryType type)
    {
        try
        {
            var inquiries = await _inquiryService.GetByTypeAsync(type);
            return Ok(new { success = true, data = inquiries, message = $"Inquiries of type '{type}' retrieved successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inquiries by type: {Type}", type);
            return StatusCode(500, new { success = false, message = "Error retrieving inquiries", error = ex.Message });
        }
    }

    [HttpGet("new")]
    public async Task<IActionResult> GetNewInquiries()
    {
        try
        {
            var inquiries = await _inquiryService.GetNewInquiriesAsync();
            return Ok(new { success = true, data = inquiries, message = "New inquiries retrieved successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving new inquiries");
            return StatusCode(500, new { success = false, message = "Error retrieving new inquiries", error = ex.Message });
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchInquiries([FromQuery] string term)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return BadRequest(new { success = false, message = "Search term is required" });
            }

            var inquiries = await _inquiryService.SearchAsync(term);
            return Ok(new { success = true, data = inquiries, message = $"Found {inquiries.Count} inquiries matching '{term}'" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching inquiries with term: {SearchTerm}", term);
            return StatusCode(500, new { success = false, message = "Error searching inquiries", error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateInquiry([FromBody] Inquiry inquiry)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid inquiry data", errors = ModelState });
            }

            await _inquiryService.CreateAsync(inquiry);
            return CreatedAtAction(nameof(GetInquiry), new { id = inquiry.Id }, 
                new { success = true, data = inquiry, message = "Inquiry created successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating inquiry");
            return StatusCode(500, new { success = false, message = "Error creating inquiry", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInquiry(string id, [FromBody] Inquiry updatedInquiry)
    {
        try
        {
            var inquiry = await _inquiryService.GetAsync(id);

            if (inquiry is null)
            {
                return NotFound(new { success = false, message = "Inquiry not found" });
            }

            updatedInquiry.Id = inquiry.Id;
            updatedInquiry.CreatedAt = inquiry.CreatedAt;

            await _inquiryService.UpdateAsync(id, updatedInquiry);
            return Ok(new { success = true, data = updatedInquiry, message = "Inquiry updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating inquiry with ID: {InquiryId}", id);
            return StatusCode(500, new { success = false, message = "Error updating inquiry", error = ex.Message });
        }
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(string id, [FromBody] StatusUpdateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid status data", errors = ModelState });
            }

            await _inquiryService.UpdateStatusAsync(id, request.Status);
            return Ok(new { success = true, message = "Status updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating status for inquiry: {InquiryId}", id);
            return StatusCode(500, new { success = false, message = "Error updating status", error = ex.Message });
        }
    }

    [HttpPost("{id}/response")]
    public async Task<IActionResult> AddResponse(string id, [FromBody] ResponseRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid response data", errors = ModelState });
            }

            await _inquiryService.AddResponseAsync(id, request.Response, request.RespondedBy);
            return Ok(new { success = true, message = "Response added successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding response to inquiry: {InquiryId}", id);
            return StatusCode(500, new { success = false, message = "Error adding response", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInquiry(string id)
    {
        try
        {
            var inquiry = await _inquiryService.GetAsync(id);

            if (inquiry is null)
            {
                return NotFound(new { success = false, message = "Inquiry not found" });
            }

            await _inquiryService.RemoveAsync(id);
            return Ok(new { success = true, message = "Inquiry deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting inquiry with ID: {InquiryId}", id);
            return StatusCode(500, new { success = false, message = "Error deleting inquiry", error = ex.Message });
        }
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetInquiryStats()
    {
        try
        {
            var totalCount = await _inquiryService.GetTotalCountAsync();
            var newCount = await _inquiryService.GetNewCountAsync();
            var pendingCount = await _inquiryService.GetPendingCountAsync();
            
            return Ok(new { 
                success = true, 
                data = new { 
                    totalCount, 
                    newCount, 
                    pendingCount
                }, 
                message = "Inquiry statistics retrieved successfully" 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inquiry statistics");
            return StatusCode(500, new { success = false, message = "Error retrieving statistics", error = ex.Message });
        }
    }
}

public class StatusUpdateRequest
{
    public InquiryStatus Status { get; set; }
}

public class ResponseRequest
{
    public string Response { get; set; } = string.Empty;
    public string RespondedBy { get; set; } = string.Empty;
}