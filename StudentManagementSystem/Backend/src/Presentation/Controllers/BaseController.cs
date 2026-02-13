using MediatR;
using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.Application.Common.Models;

namespace StudentManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected readonly IMediator _mediator;

    protected BaseController(IMediator mediator)
    {
        _mediator = mediator;
    }

    protected ActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(new ApiResponse<T>
            {
                Success = true,
                Message = result.Message,
                Data = result.Data
            });
        }

        return BadRequest(new ApiResponse<T>
        {
            Success = false,
            Message = result.Message,
            Errors = result.Errors
        });
    }

    protected ActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return Ok(new ApiResponse
            {
                Success = true,
                Message = result.Message
            });
        }

        return BadRequest(new ApiResponse
        {
            Success = false,
            Message = result.Message,
            Errors = result.Errors
        });
    }

    protected ActionResult HandlePagedResult<T>(Result<PagedResult<T>> result)
    {
        if (result.IsSuccess)
        {
            return Ok(new ApiPagedResponse<T>
            {
                Success = true,
                Message = result.Message,
                Data = result.Data!.Data,
                TotalCount = result.Data.TotalCount,
                PageNumber = result.Data.PageNumber,
                PageSize = result.Data.PageSize,
                TotalPages = result.Data.TotalPages,
                HasNextPage = result.Data.HasNextPage,
                HasPreviousPage = result.Data.HasPreviousPage
            });
        }

        return BadRequest(new ApiResponse<IEnumerable<T>>
        {
            Success = false,
            Message = result.Message,
            Errors = result.Errors
        });
    }
}

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }
}

public class ApiPagedResponse<T> : ApiResponse<IEnumerable<T>>
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}