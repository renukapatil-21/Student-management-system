using MediatR;
using AutoMapper;
using StudentManagementSystem.Application.Common.Models;
using StudentManagementSystem.Application.DTOs.Students;
using StudentManagementSystem.Domain.Interfaces;
using StudentManagementSystem.Domain.Enums;
using System.Linq.Expressions;
using StudentManagementSystem.Domain.Entities;

namespace StudentManagementSystem.Application.Features.Students.Queries.GetStudents;

public class GetStudentsQuery : IRequest<Result<PagedResult<StudentDto>>>
{
    public PaginationRequest PaginationRequest { get; set; } = new();
    public StudentSearchDto? SearchCriteria { get; set; }
}

public class GetStudentsQueryHandler : IRequestHandler<GetStudentsQuery, Result<PagedResult<StudentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetStudentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<StudentDto>>> Handle(GetStudentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            Expression<Func<Student, bool>>? filter = null;

            if (request.SearchCriteria != null)
            {
                filter = BuildFilterExpression(request.SearchCriteria);
            }

            var students = await _unitOfWork.Students.GetAllAsync();
            
            if (filter != null)
            {
                // Apply filter if needed - for now, get all students
                students = students.Where(filter.Compile());
            }

            var totalCount = students.Count();
            
            // Apply pagination
            var pagedStudents = students
                .Skip((request.PaginationRequest.PageNumber - 1) * request.PaginationRequest.PageSize)
                .Take(request.PaginationRequest.PageSize);

            var studentDtos = _mapper.Map<IEnumerable<StudentDto>>(pagedStudents);

            var pagedResult = new PagedResult<StudentDto>
            {
                Data = studentDtos,
                TotalCount = totalCount,
                PageNumber = request.PaginationRequest.PageNumber,
                PageSize = request.PaginationRequest.PageSize
            };

            return Result<PagedResult<StudentDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<StudentDto>>.Failure($"Error retrieving students: {ex.Message}");
        }
    }

    private Expression<Func<Student, bool>> BuildFilterExpression(StudentSearchDto criteria)
    {
        Expression<Func<Student, bool>> filter = s => true;

        if (!string.IsNullOrEmpty(criteria.StudentId))
        {
            filter = CombineExpressions(filter, s => s.StudentId.Contains(criteria.StudentId));
        }

        if (!string.IsNullOrEmpty(criteria.Name))
        {
            filter = CombineExpressions(filter, s => 
                s.FirstName.Contains(criteria.Name) || s.LastName.Contains(criteria.Name));
        }

        if (!string.IsNullOrEmpty(criteria.Email))
        {
            filter = CombineExpressions(filter, s => s.Email.Contains(criteria.Email));
        }

        if (!string.IsNullOrEmpty(criteria.Department))
        {
            filter = CombineExpressions(filter, s => s.Department == criteria.Department);
        }

        if (criteria.AcademicYear.HasValue)
        {
            filter = CombineExpressions(filter, s => s.AcademicYear == criteria.AcademicYear.Value);
        }

        if (criteria.Status.HasValue)
        {
            filter = CombineExpressions(filter, s => s.Status == criteria.Status.Value);
        }

        return filter;
    }

    private Expression<Func<Student, bool>> CombineExpressions(
        Expression<Func<Student, bool>> expr1,
        Expression<Func<Student, bool>> expr2)
    {
        var parameter = Expression.Parameter(typeof(Student), "s");
        var body = Expression.AndAlso(
            Expression.Invoke(expr1, parameter),
            Expression.Invoke(expr2, parameter)
        );
        return Expression.Lambda<Func<Student, bool>>(body, parameter);
    }
}