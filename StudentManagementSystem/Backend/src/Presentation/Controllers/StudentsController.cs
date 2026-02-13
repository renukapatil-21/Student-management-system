using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.Application.Features.Students.Commands.CreateStudent;
using StudentManagementSystem.Application.Features.Students.Commands.UpdateStudent;
using StudentManagementSystem.Application.Features.Students.Commands.DeleteStudent;
using StudentManagementSystem.Application.Features.Students.Queries.GetStudents;
using StudentManagementSystem.Application.Features.Students.Queries.GetStudentById;
using StudentManagementSystem.Application.DTOs.Students;
using StudentManagementSystem.Application.Common.Models;

namespace StudentManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentsController : BaseController
{
    public StudentsController(IMediator mediator) : base(mediator) { }

    [HttpGet]
    public async Task<ActionResult> GetStudents(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "CreatedAt",
        [FromQuery] bool sortDescending = true,
        [FromQuery] string? studentId = null,
        [FromQuery] string? name = null,
        [FromQuery] string? email = null,
        [FromQuery] string? department = null)
    {
        var searchCriteria = new StudentSearchDto
        {
            StudentId = studentId,
            Name = name,
            Email = email,
            Department = department
        };

        var paginationRequest = new PaginationRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDescending = sortDescending
        };

        var query = new GetStudentsQuery
        {
            PaginationRequest = paginationRequest,
            SearchCriteria = searchCriteria
        };

        var result = await _mediator.Send(query);
        return HandlePagedResult(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetStudent(string id)
    {
        var query = new GetStudentByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<ActionResult> CreateStudent([FromBody] CreateStudentDto studentDto)
    {
        var command = new CreateStudentCommand { Student = studentDto };
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<ActionResult> UpdateStudent(string id, [FromBody] UpdateStudentDto studentDto)
    {
        var command = new UpdateStudentCommand { Id = id, Student = studentDto };
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteStudent(string id)
    {
        var command = new DeleteStudentCommand { Id = id };
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }
}