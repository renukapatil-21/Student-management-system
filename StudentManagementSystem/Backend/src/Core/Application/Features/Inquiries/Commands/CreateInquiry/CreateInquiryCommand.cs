using MediatR;
using AutoMapper;
using StudentManagementSystem.Application.Common.Models;
using StudentManagementSystem.Application.DTOs.Inquiries;
using StudentManagementSystem.Domain.Interfaces;
using StudentManagementSystem.Domain.Entities;
using StudentManagementSystem.Domain.Enums;
using FluentValidation;

namespace StudentManagementSystem.Application.Features.Inquiries.Commands.CreateInquiry;

public class CreateInquiryCommand : IRequest<Result<InquiryDto>>
{
    public CreateInquiryDto Inquiry { get; set; } = new();
}

public class CreateInquiryCommandValidator : AbstractValidator<CreateInquiryCommand>
{
    public CreateInquiryCommandValidator()
    {
        RuleFor(x => x.Inquiry.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Inquiry.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

        RuleFor(x => x.Inquiry.Category)
            .NotEmpty().WithMessage("Category is required");

        RuleFor(x => x.Inquiry.InquirerName)
            .NotEmpty().WithMessage("Inquirer name is required")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

        RuleFor(x => x.Inquiry.InquirerEmail)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Inquiry.InquirerPhone)
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^[+]?[\d\s\-\(\)]{10,15}$").WithMessage("Invalid phone number format");

        RuleFor(x => x.Inquiry.Priority)
            .IsInEnum().WithMessage("Invalid priority");
    }
}

public class CreateInquiryCommandHandler : IRequestHandler<CreateInquiryCommand, Result<InquiryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateInquiryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<InquiryDto>> Handle(CreateInquiryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var inquiry = _mapper.Map<Inquiry>(request.Inquiry);
            inquiry.Status = InquiryStatus.New;

            await _unitOfWork.Inquiries.CreateAsync(inquiry);
            
            if (await _unitOfWork.SaveChangesAsync())
            {
                var inquiryDto = _mapper.Map<InquiryDto>(inquiry);
                return Result<InquiryDto>.Success(inquiryDto, "Inquiry created successfully");
            }

            return Result<InquiryDto>.Failure("Failed to create inquiry");
        }
        catch (Exception ex)
        {
            return Result<InquiryDto>.Failure($"Error creating inquiry: {ex.Message}");
        }
    }
}