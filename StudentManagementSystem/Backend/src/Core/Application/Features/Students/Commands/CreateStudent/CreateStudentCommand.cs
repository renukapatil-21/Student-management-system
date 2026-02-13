using MediatR;
using AutoMapper;
using StudentManagementSystem.Application.Common.Models;
using StudentManagementSystem.Application.DTOs.Students;
using StudentManagementSystem.Domain.Interfaces;
using StudentManagementSystem.Domain.Entities;
using FluentValidation;

namespace StudentManagementSystem.Application.Features.Students.Commands.CreateStudent;

public class CreateStudentCommand : IRequest<Result<StudentDto>>
{
    public CreateStudentDto Student { get; set; } = new();
}

public class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
{
    public CreateStudentCommandValidator()
    {
        RuleFor(x => x.Student.StudentId)
            .NotEmpty().WithMessage("Student ID is required")
            .MaximumLength(20).WithMessage("Student ID cannot exceed 20 characters");

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

        RuleFor(x => x.Student.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .Must(BeAValidAge).WithMessage("Student must be between 15 and 40 years old");

        RuleFor(x => x.Student.Department)
            .NotEmpty().WithMessage("Department is required");

        RuleFor(x => x.Student.Course)
            .NotEmpty().WithMessage("Course is required");

        RuleFor(x => x.Student.AdmissionDate)
            .NotEmpty().WithMessage("Admission date is required")
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Admission date cannot be in the future");
    }

    private bool BeAValidAge(DateTime dateOfBirth)
    {
        var age = DateTime.Now.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > DateTime.Now.AddYears(-age))
            age--;
        return age >= 15 && age <= 40;
    }
}

public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, Result<StudentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateStudentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<StudentDto>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if student ID already exists
            if (await _unitOfWork.Students.StudentIdExistsAsync(request.Student.StudentId))
            {
                return Result<StudentDto>.Failure("Student ID already exists");
            }

            // Check if email already exists
            if (await _unitOfWork.Students.EmailExistsAsync(request.Student.Email))
            {
                return Result<StudentDto>.Failure("Email already exists");
            }

            var student = _mapper.Map<Student>(request.Student);
            student.Status = Domain.Enums.StudentStatus.Active;

            await _unitOfWork.Students.CreateAsync(student);
            
            if (await _unitOfWork.SaveChangesAsync())
            {
                var studentDto = _mapper.Map<StudentDto>(student);
                return Result<StudentDto>.Success(studentDto, "Student created successfully");
            }

            return Result<StudentDto>.Failure("Failed to create student");
        }
        catch (Exception ex)
        {
            return Result<StudentDto>.Failure($"Error creating student: {ex.Message}");
        }
    }
}