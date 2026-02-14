using StudentAPI.Models;

namespace StudentAPI.Services
{
    public interface IStudentService
    {
        Task<List<Student>> GetAsync();
        Task<Student?> GetAsync(string id);
        Task CreateAsync(Student student);
        Task UpdateAsync(string id, Student student);
        Task RemoveAsync(string id);
        Task<List<Student>> SearchAsync(string searchTerm);
        Task<long> GetTotalCountAsync();
        Task<List<Student>> GetRecentStudentsAsync(int limit);
    }
}