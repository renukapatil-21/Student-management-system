using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.Application.Features.Inquiries.Commands.CreateInquiry;
using StudentManagementSystem.Application.DTOs.Inquiries;

namespace StudentManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InquiriesController : BaseController
{
    public InquiriesController(IMediator mediator) : base(mediator) { }

    [HttpPost]
    public async Task<ActionResult> CreateInquiry([FromBody] CreateInquiryDto inquiryDto)
    {
        var command = new CreateInquiryCommand { Inquiry = inquiryDto };
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<ActionResult> GetInquiries()
    {
        // Implementation would use a query to get inquiries
        return Ok(new { Message = "Get inquiries implementation needed" });
    }

    [HttpPut("{id}/assign")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<ActionResult> AssignInquiry(string id, [FromBody] AssignInquiryDto assignDto)
    {
        // Implementation would use a command to assign inquiry
        return Ok(new { Message = "Assign inquiry implementation needed" });
    }

    [HttpPut("{id}/resolve")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<ActionResult> ResolveInquiry(string id, [FromBody] ResolveInquiryDto resolveDto)
    {
        // Implementation would use a command to resolve inquiry
        return Ok(new { Message = "Resolve inquiry implementation needed" });
    }
}