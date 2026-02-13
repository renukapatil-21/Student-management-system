using AutoMapper;
using StudentManagementSystem.Domain.Entities;
using StudentManagementSystem.Application.DTOs.Users;
using StudentManagementSystem.Application.DTOs.Students;
using StudentManagementSystem.Application.DTOs.Fees;
using StudentManagementSystem.Application.DTOs.Inquiries;

namespace StudentManagementSystem.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<CreateUserDto, User>();
        CreateMap<UpdateUserDto, User>();

        CreateMap<Student, StudentDto>().ReverseMap();
        CreateMap<CreateStudentDto, Student>();
        CreateMap<UpdateStudentDto, Student>();

        CreateMap<Fee, FeeDto>().ReverseMap();
        CreateMap<CreateFeeDto, Fee>();
        CreateMap<UpdateFeeDto, Fee>();

        CreateMap<Inquiry, InquiryDto>().ReverseMap();
        CreateMap<CreateInquiryDto, Inquiry>();
        CreateMap<UpdateInquiryDto, Inquiry>();

        CreateMap<DashboardAnalytics, DashboardAnalyticsDto>().ReverseMap();
    }
}