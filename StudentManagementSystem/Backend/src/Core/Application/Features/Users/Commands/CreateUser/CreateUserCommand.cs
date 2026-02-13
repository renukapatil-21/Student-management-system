using MediatR;
using AutoMapper;
using StudentManagementSystem.Application.Common.Models;
using StudentManagementSystem.Application.DTOs.Users;
using StudentManagementSystem.Domain.Interfaces;
using StudentManagementSystem.Domain.Entities;
using FluentValidation;
using BCrypt.Net;

namespace StudentManagementSystem.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommand : IRequest<Result<UserDto>>
{
    public CreateUserDto User { get; set; } = new();
}

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.User.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.User.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("Password must contain at least one uppercase letter, one lowercase letter, and one number");

        RuleFor(x => x.User.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

        RuleFor(x => x.User.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

        RuleFor(x => x.User.Role)
            .IsInEnum().WithMessage("Invalid user role");
    }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if email already exists
            if (await _unitOfWork.Users.EmailExistsAsync(request.User.Email))
            {
                return Result<UserDto>.Failure("Email already exists");
            }

            var user = _mapper.Map<User>(request.User);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.User.Password);

            await _unitOfWork.Users.CreateAsync(user);
            
            if (await _unitOfWork.SaveChangesAsync())
            {
                var userDto = _mapper.Map<UserDto>(user);
                return Result<UserDto>.Success(userDto, "User created successfully");
            }

            return Result<UserDto>.Failure("Failed to create user");
        }
        catch (Exception ex)
        {
            return Result<UserDto>.Failure($"Error creating user: {ex.Message}");
        }
    }
}