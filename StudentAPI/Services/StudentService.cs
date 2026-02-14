using MongoDB.Driver;
using StudentAPI.Models;
using Microsoft.Extensions.Options;

namespace StudentAPI.Services;

public class StudentService : IStudentService
{
    private readonly IMongoCollection<Student> _studentsCollection;

    public StudentService(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _studentsCollection = mongoDatabase.GetCollection<Student>(mongoDbSettings.Value.StudentsCollectionName);
    }

    public async Task<List<Student>> GetAsync() =>
        await _studentsCollection.Find(_ => true).ToListAsync();

    public async Task<Student?> GetAsync(string id) =>
        await _studentsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<List<Student>> SearchAsync(string searchTerm)
    {
        var filter = Builders<Student>.Filter.Or(
            Builders<Student>.Filter.Regex(s => s.FirstName, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
            Builders<Student>.Filter.Regex(s => s.LastName, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
            Builders<Student>.Filter.Regex(s => s.Email, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
            Builders<Student>.Filter.Regex(s => s.Course, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
        );

        return await _studentsCollection.Find(filter).ToListAsync();
    }

    public async Task CreateAsync(Student newStudent)
    {
        newStudent.CreatedAt = DateTime.UtcNow;
        newStudent.UpdatedAt = DateTime.UtcNow;
        await _studentsCollection.InsertOneAsync(newStudent);
    }

    public async Task UpdateAsync(string id, Student updatedStudent)
    {
        updatedStudent.UpdatedAt = DateTime.UtcNow;
        await _studentsCollection.ReplaceOneAsync(x => x.Id == id, updatedStudent);
    }

    public async Task RemoveAsync(string id) =>
        await _studentsCollection.DeleteOneAsync(x => x.Id == id);

    public async Task<long> GetTotalCountAsync() =>
        await _studentsCollection.CountDocumentsAsync(_ => true);

    public async Task<List<Student>> GetRecentStudentsAsync(int count = 5) =>
        await _studentsCollection.Find(_ => true)
            .SortByDescending(s => s.CreatedAt)
            .Limit(count)
            .ToListAsync();
}