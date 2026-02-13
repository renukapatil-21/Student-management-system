using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.Application.Features.Fees.Commands.CreateFee;
using StudentManagementSystem.Application.Features.Fees.Commands.PayFee;
using StudentManagementSystem.Application.DTOs.Fees;

namespace StudentManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FeesController : BaseController
{
    public FeesController(IMediator mediator) : base(mediator) { }

    [HttpPost]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<ActionResult> CreateFee([FromBody] CreateFeeDto feeDto)
    {
        var command = new CreateFeeCommand { Fee = feeDto };
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPost("pay")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<ActionResult> PayFee([FromBody] PayFeeDto paymentDto)
    {
        var command = new PayFeeCommand { PaymentData = paymentDto };
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet("student/{studentId}")]
    public async Task<ActionResult> GetStudentFees(string studentId)
    {
        // Implementation would use a query to get student fees
        return Ok(new { Message = "Get student fees implementation needed" });
    }

    [HttpGet("summary")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<ActionResult> GetFeesSummary()
    {
        // Implementation would use a query to get fees summary
        return Ok(new { Message = "Get fees summary implementation needed" });
    }
}