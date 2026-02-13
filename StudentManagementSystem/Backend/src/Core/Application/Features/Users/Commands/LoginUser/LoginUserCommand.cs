using MediatR;
using AutoMapper;
using StudentManagementSystem.Application.Common.Models;
using StudentManagementSystem.Application.DTOs.Users;
using StudentManagementSystem.Domain.Interfaces;
using StudentManagementSystem.Domain.Entities;
using FluentValidation;
using BCrypt.Net;

namespace StudentManagementSystem.Application.Features.Users.Commands.LoginUser;

public class LoginUserCommand : IRequest<Result<UserLoginResponseDto>>
{
    public UserLoginDto LoginData { get; set; } = new();
}

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.LoginData.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.LoginData.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<UserLoginResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IJwtTokenGenerator jwtTokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<UserLoginResponseDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(request.LoginData.Email);
            if (user == null)
            {
                return Result<UserLoginResponseDto>.Failure("Invalid email or password");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.LoginData.Password, user.PasswordHash))
            {
                return Result<UserLoginResponseDto>.Failure("Invalid email or password");
            }

            var token = _jwtTokenGenerator.GenerateToken(user);
            var userDto = _mapper.Map<UserDto>(user);

            var response = new UserLoginResponseDto
            {
                User = userDto,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            return Result<UserLoginResponseDto>.Success(response, "Login successful");
        }
        catch (Exception ex)
        {
            return Result<UserLoginResponseDto>.Failure($"Error during login: {ex.Message}");
        }
    }
}

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}