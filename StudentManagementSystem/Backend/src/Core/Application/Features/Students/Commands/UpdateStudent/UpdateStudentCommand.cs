using MediatR;
using AutoMapper;
using StudentManagementSystem.Application.Common.Models;
using StudentManagementSystem.Application.DTOs.Students;
using StudentManagementSystem.Domain.Interfaces;
using FluentValidation;

namespace StudentManagementSystem.Application.Features.Students.Commands.UpdateStudent;

public class UpdateStudentCommand : IRequest<Result<StudentDto>>
{
    public string Id { get; set; } = string.Empty;
    public UpdateStudentDto Student { get; set; } = new();
}

public class UpdateStudentCommandValidator : AbstractValidator<UpdateStudentCommand>
{
    public UpdateStudentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Student ID is required");

        RuleFor(x => x.Student.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

        RuleFor(x => x.Student.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

        RuleFor(x => x.Student.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Student.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^[+]?[\d\s\-\(\)]{10,15}$").WithMessage("Invalid phone number format");

        RuleFor(x => x.Student.Department)
            .NotEmpty().WithMessage("Department is required");

        RuleFor(x => x.Student.Course)
            .NotEmpty().WithMessage("Course is required");
    }
}

public class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, Result<StudentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateStudentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<StudentDto>> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingStudent = await _unitOfWork.Students.GetByIdAsync(request.Id);
            if (existingStudent == null)
            {
                return Result<StudentDto>.Failure("Student not found");
            }

            // Check if email already exists for another student
            var studentWithEmail = await _unitOfWork.Students.GetByEmailAsync(request.Student.Email);
            if (studentWithEmail != null && studentWithEmail.Id != request.Id)
            {
                return Result<StudentDto>.Failure("Email already exists for another student");
            }

            _mapper.Map(request.Student, existingStudent);
            existingStudent.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Students.UpdateAsync(existingStudent);
            
            if (await _unitOfWork.SaveChangesAsync())
            {
                var studentDto = _mapper.Map<StudentDto>(existingStudent);
                return Result<StudentDto>.Success(studentDto, "Student updated successfully");
            }

            return Result<StudentDto>.Failure("Failed to update student");
        }
        catch (Exception ex)
        {
            return Result<StudentDto>.Failure($"Error updating student: {ex.Message}");
        }
    }
}