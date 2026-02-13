using MediatR;
using AutoMapper;
using StudentManagementSystem.Application.Common.Models;
using StudentManagementSystem.Application.DTOs.Fees;
using StudentManagementSystem.Domain.Interfaces;
using StudentManagementSystem.Domain.Enums;
using FluentValidation;

namespace StudentManagementSystem.Application.Features.Fees.Commands.PayFee;

public class PayFeeCommand : IRequest<Result<FeeDto>>
{
    public PayFeeDto PaymentData { get; set; } = new();
}

public class PayFeeCommandValidator : AbstractValidator<PayFeeCommand>
{
    public PayFeeCommandValidator()
    {
        RuleFor(x => x.PaymentData.FeeId)
            .NotEmpty().WithMessage("Fee ID is required");

        RuleFor(x => x.PaymentData.PaymentMethod)
            .IsInEnum().WithMessage("Invalid payment method");

        RuleFor(x => x.PaymentData.PaymentDate)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Payment date cannot be in the future");
    }
}

public class PayFeeCommandHandler : IRequestHandler<PayFeeCommand, Result<FeeDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PayFeeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<FeeDto>> Handle(PayFeeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var fee = await _unitOfWork.Fees.GetByIdAsync(request.PaymentData.FeeId);
            if (fee == null)
            {
                return Result<FeeDto>.Failure("Fee not found");
            }

            if (fee.Status == FeeStatus.Paid)
            {
                return Result<FeeDto>.Failure("Fee is already paid");
            }

            fee.Status = FeeStatus.Paid;
            fee.PaidDate = request.PaymentData.PaymentDate;
            fee.PaymentMethod = request.PaymentData.PaymentMethod;
            fee.TransactionId = request.PaymentData.TransactionId;
            fee.PaymentNotes = request.PaymentData.PaymentRemarks;
            fee.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Fees.UpdateAsync(fee);
            
            if (await _unitOfWork.SaveChangesAsync())
            {
                var student = await _unitOfWork.Students.GetByIdAsync(fee.StudentId);
                var feeDto = _mapper.Map<FeeDto>(fee);
                feeDto.StudentName = student != null ? $"{student.FirstName} {student.LastName}" : "";
                
                return Result<FeeDto>.Success(feeDto, "Fee payment recorded successfully");
            }

            return Result<FeeDto>.Failure("Failed to record fee payment");
        }
        catch (Exception ex)
        {
            return Result<FeeDto>.Failure($"Error recording fee payment: {ex.Message}");
        }
    }
}