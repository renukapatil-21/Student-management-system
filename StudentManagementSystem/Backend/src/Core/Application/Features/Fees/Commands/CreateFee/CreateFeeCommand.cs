using MediatR;
using AutoMapper;
using StudentManagementSystem.Application.Common.Models;
using StudentManagementSystem.Application.DTOs.Fees;
using StudentManagementSystem.Domain.Interfaces;
using StudentManagementSystem.Domain.Entities;
using StudentManagementSystem.Domain.Enums;
using FluentValidation;

namespace StudentManagementSystem.Application.Features.Fees.Commands.CreateFee;

public class CreateFeeCommand : IRequest<Result<FeeDto>>
{
    public CreateFeeDto Fee { get; set; } = new();
}

public class CreateFeeCommandValidator : AbstractValidator<CreateFeeCommand>
{
    public CreateFeeCommandValidator()
    {
        RuleFor(x => x.Fee.StudentId)
            .NotEmpty().WithMessage("Student ID is required");

        RuleFor(x => x.Fee.FeeType)
            .NotEmpty().WithMessage("Fee type is required")
            .MaximumLength(100).WithMessage("Fee type cannot exceed 100 characters");

        RuleFor(x => x.Fee.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0");

        RuleFor(x => x.Fee.DueDate)
            .GreaterThan(DateTime.Today).WithMessage("Due date must be in the future");

        RuleFor(x => x.Fee.AcademicYear)
            .IsInEnum().WithMessage("Invalid academic year");
    }
}

public class CreateFeeCommandHandler : IRequestHandler<CreateFeeCommand, Result<FeeDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateFeeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<FeeDto>> Handle(CreateFeeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verify student exists
            var student = await _unitOfWork.Students.GetByIdAsync(request.Fee.StudentId);
            if (student == null)
            {
                return Result<FeeDto>.Failure("Student not found");
            }

            var fee = _mapper.Map<Fee>(request.Fee);
            fee.Status = FeeStatus.Pending;

            await _unitOfWork.Fees.CreateAsync(fee);
            
            if (await _unitOfWork.SaveChangesAsync())
            {
                var feeDto = _mapper.Map<FeeDto>(fee);
                feeDto.StudentName = $"{student.FirstName} {student.LastName}";
                return Result<FeeDto>.Success(feeDto, "Fee created successfully");
            }

            return Result<FeeDto>.Failure("Failed to create fee");
        }
        catch (Exception ex)
        {
            return Result<FeeDto>.Failure($"Error creating fee: {ex.Message}");
        }
    }
}