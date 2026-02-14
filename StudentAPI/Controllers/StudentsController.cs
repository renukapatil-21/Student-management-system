using Microsoft.AspNetCore.Mvc;
using StudentAPI.Models;
using StudentAPI.Services;

namespace StudentAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly ILogger<StudentsController> _logger;
    private readonly IStudentService _studentService;

    public StudentsController(ILogger<StudentsController> logger, IStudentService studentService)
    {
        _logger = logger;
        _studentService = studentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStudents()
    {
        try
        {
            var students = await _studentService.GetAsync();
            return Ok(new { success = true, data = students, message = "Students retrieved successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving students");
            return StatusCode(500, new { success = false, message = "Error retrieving students", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetStudent(string id)
    {
        try
        {
            var student = await _studentService.GetAsync(id);
            
            if (student is null)
            {
                return NotFound(new { success = false, message = "Student not found" });
            }

            return Ok(new { success = true, data = student, message = "Student retrieved successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student with ID: {StudentId}", id);
            return StatusCode(500, new { success = false, message = "Error retrieving student", error = ex.Message });
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchStudents([FromQuery] string term)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return BadRequest(new { success = false, message = "Search term is required" });
            }

            var students = await _studentService.SearchAsync(term);
            return Ok(new { success = true, data = students, message = $"Found {students.Count} students matching '{term}'" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching students with term: {SearchTerm}", term);
            return StatusCode(500, new { success = false, message = "Error searching students", error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateStudent([FromBody] Student student)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid student data", errors = ModelState });
            }

            await _studentService.CreateAsync(student);
            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, 
                new { success = true, data = student, message = "Student created successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating student");
            return StatusCode(500, new { success = false, message = "Error creating student", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStudent(string id, [FromBody] Student updatedStudent)
    {
        try
        {
            var student = await _studentService.GetAsync(id);

            if (student is null)
            {
                return NotFound(new { success = false, message = "Student not found" });
            }

            updatedStudent.Id = student.Id;
            updatedStudent.CreatedAt = student.CreatedAt; // Preserve creation date

            await _studentService.UpdateAsync(id, updatedStudent);
            return Ok(new { success = true, data = updatedStudent, message = "Student updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating student with ID: {StudentId}", id);
            return StatusCode(500, new { success = false, message = "Error updating student", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStudent(string id)
    {
        try
        {
            var student = await _studentService.GetAsync(id);

            if (student is null)
            {
                return NotFound(new { success = false, message = "Student not found" });
            }

            await _studentService.RemoveAsync(id);
            return Ok(new { success = true, message = "Student deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting student with ID: {StudentId}", id);
            return StatusCode(500, new { success = false, message = "Error deleting student", error = ex.Message });
        }
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStudentStats()
    {
        try
        {
            var totalCount = await _studentService.GetTotalCountAsync();
            var recentStudents = await _studentService.GetRecentStudentsAsync(5);
            
            return Ok(new { 
                success = true, 
                data = new { 
                    totalCount, 
                    recentCount = recentStudents.Count, 
                    recentStudents 
                }, 
                message = "Student statistics retrieved successfully" 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student statistics");
            return StatusCode(500, new { success = false, message = "Error retrieving statistics", error = ex.Message });
        }
    }
}