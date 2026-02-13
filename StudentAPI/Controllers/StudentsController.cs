using Microsoft.AspNetCore.Mvc;

namespace StudentAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(ILogger<StudentsController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetStudents()
    {
        var students = new[]
        {
            new { id = "1", firstName = "John", lastName = "Doe", email = "john@example.com", studentId = "STU001", status = "Active" },
            new { id = "2", firstName = "Jane", lastName = "Smith", email = "jane@example.com", studentId = "STU002", status = "Active" },
            new { id = "3", firstName = "Bob", lastName = "Johnson", email = "bob@example.com", studentId = "STU003", status = "Inactive" },
        };

        return Ok(new { success = true, data = students, message = "Students retrieved successfully" });
    }

    [HttpGet("{id}")]
    public IActionResult GetStudent(string id)
    {
        var student = new { id, firstName = "John", lastName = "Doe", email = "john@example.com", studentId = "STU001", status = "Active" };
        return Ok(new { success = true, data = student, message = "Student retrieved successfully" });
    }

    [HttpPost]
    public IActionResult CreateStudent([FromBody] object studentData)
    {
        return Ok(new { success = true, data = studentData, message = "Student created successfully" });
    }
}