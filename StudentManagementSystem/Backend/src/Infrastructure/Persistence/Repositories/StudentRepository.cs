using MongoDB.Driver;
using MongoDB.Bson;
using StudentManagementSystem.Domain.Entities;
using StudentManagementSystem.Domain.Interfaces;
using StudentManagementSystem.Domain.Enums;
using StudentManagementSystem.Application.Interfaces.Repositories;

namespace StudentManagementSystem.Infrastructure.Persistence.Repositories;

public class StudentRepository : GenericRepository<Student>, IStudentRepository
{
    public StudentRepository(IMongoDatabase database) : base(database)
    {
    }

    public async Task<IEnumerable<Student>> GetByStatusAsync(StudentStatus status)
    {
        return await _collection.Find(s => s.Status == status).ToListAsync();
    }

    public async Task<IEnumerable<Student>> GetByClassAsync(string className)
    {
        return await _collection.Find(s => s.Class == className).ToListAsync();
    }

    public async Task<IEnumerable<Student>> GetByAcademicYearAsync(AcademicYear academicYear)
    {
        return await _collection.Find(s => s.AcademicYear == academicYear).ToListAsync();
    }

    public async Task<IEnumerable<Student>> GetActiveStudentsAsync()
    {
        return await _collection.Find(s => s.Status == StudentStatus.Active).ToListAsync();
    }

    public async Task<int> GetStudentCountByStatusAsync(StudentStatus status)
    {
        return (int)await _collection.CountDocumentsAsync(s => s.Status == status);
    }

    public async Task<IEnumerable<Student>> SearchStudentsAsync(string searchTerm)
    {
        var filter = Builders<Student>.Filter.Or(
            Builders<Student>.Filter.Regex(s => s.FirstName, new BsonRegularExpression(searchTerm, "i")),
            Builders<Student>.Filter.Regex(s => s.LastName, new BsonRegularExpression(searchTerm, "i")),
            Builders<Student>.Filter.Regex(s => s.RollNumber, new BsonRegularExpression(searchTerm, "i"))
        );
        
        return await _collection.Find(filter).ToListAsync();
    }
}