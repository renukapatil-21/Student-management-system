using MediatR;
using StudentManagementSystem.Application.Common.Models;
using StudentManagementSystem.Domain.Interfaces;
using FluentValidation;

namespace StudentManagementSystem.Application.Features.Students.Commands.DeleteStudent;

public class DeleteStudentCommand : IRequest<Result>
{
    public string Id { get; set; } = string.Empty;
}

public class DeleteStudentCommandValidator : AbstractValidator<DeleteStudentCommand>
{
    public DeleteStudentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Student ID is required");
    }
}

public class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteStudentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(request.Id);
            if (student == null)
            {
                return Result.Failure("Student not found");
            }

            // Check if student has any fees
            var studentFees = await _unitOfWork.Fees.GetByStudentIdAsync(request.Id);
            if (studentFees.Any())
            {
                return Result.Failure("Cannot delete student with existing fee records. Please archive the student instead.");
            }

            await _unitOfWork.Students.DeleteAsync(request.Id);
            
            if (await _unitOfWork.SaveChangesAsync())
            {
                return Result.Success("Student deleted successfully");
            }

            return Result.Failure("Failed to delete student");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error deleting student: {ex.Message}");
        }
    }
}