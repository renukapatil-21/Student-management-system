using MediatR;
using AutoMapper;
using StudentManagementSystem.Application.Common.Models;
using StudentManagementSystem.Application.DTOs.Students;
using StudentManagementSystem.Domain.Interfaces;
using FluentValidation;

namespace StudentManagementSystem.Application.Features.Students.Queries.GetStudentById;

public class GetStudentByIdQuery : IRequest<Result<StudentDto>>
{
    public string Id { get; set; } = string.Empty;
}

public class GetStudentByIdQueryValidator : AbstractValidator<GetStudentByIdQuery>
{
    public GetStudentByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Student ID is required");
    }
}

public class GetStudentByIdQueryHandler : IRequestHandler<GetStudentByIdQuery, Result<StudentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetStudentByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<StudentDto>> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(request.Id);
            if (student == null)
            {
                return Result<StudentDto>.Failure("Student not found");
            }

            var studentDto = _mapper.Map<StudentDto>(student);
            return Result<StudentDto>.Success(studentDto);
        }
        catch (Exception ex)
        {
            return Result<StudentDto>.Failure($"Error retrieving student: {ex.Message}");
        }
    }
}