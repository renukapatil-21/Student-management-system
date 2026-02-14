using StudentAPI.Models;
using StudentAPI.Services;
using System.Collections.Concurrent;

namespace StudentAPI.Services
{
    public class InMemoryStudentService : IStudentService
    {
        private readonly ConcurrentDictionary<string, Student> _students = new();
        
        public InMemoryStudentService()
        {
            // Seed with sample data
            SeedData();
        }
        
        private void SeedData()
        {
            var students = new[]
            {
                new Student
                {
                    Id = "1",
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    Phone = "+1234567890",
                    DateOfBirth = DateTime.Parse("2000-01-15"),
                    Address = new Address
                    {
                        Street = "123 Main St",
                        City = "Springfield",
                        State = "IL",
                        ZipCode = "62701",
                        Country = "USA"
                    },
                    GuardianInfo = new GuardianInfo
                    {
                        Name = "Jane Doe",
                        Phone = "+1234567891",
                        Relationship = "Mother"
                    },
                    EnrollmentDate = DateTime.UtcNow.AddMonths(-6),
                    IsActive = true
                },
                new Student
                {
                    Id = "2",
                    FirstName = "Alice",
                    LastName = "Smith",
                    Email = "alice.smith@example.com",
                    Phone = "+1234567892",
                    DateOfBirth = DateTime.Parse("1999-08-22"),
                    Address = new Address
                    {
                        Street = "456 Oak Ave",
                        City = "Chicago",
                        State = "IL",
                        ZipCode = "60601",
                        Country = "USA"
                    },
                    GuardianInfo = new GuardianInfo
                    {
                        Name = "Robert Smith",
                        Phone = "+1234567893",
                        Relationship = "Father"
                    },
                    EnrollmentDate = DateTime.UtcNow.AddMonths(-4),
                    IsActive = true
                }
            };
            
            foreach (var student in students)
            {
                _students.TryAdd(student.Id, student);
            }
        }

        public async Task<List<Student>> GetAsync()
        {
            return await Task.FromResult(_students.Values.ToList());
        }

        public async Task<Student?> GetAsync(string id)
        {
            _students.TryGetValue(id, out var student);
            return await Task.FromResult(student);
        }

        public async Task CreateAsync(Student student)
        {
            student.Id = Guid.NewGuid().ToString();
            _students.TryAdd(student.Id, student);
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(string id, Student student)
        {
            _students.TryUpdate(id, student, _students[id]);
            await Task.CompletedTask;
        }

        public async Task RemoveAsync(string id)
        {
            _students.TryRemove(id, out _);
            await Task.CompletedTask;
        }

        public async Task<List<Student>> SearchAsync(string searchTerm)
        {
            var students = _students.Values
                .Where(s => s.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           s.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           s.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
            
            return await Task.FromResult(students);
        }

        public async Task<long> GetTotalCountAsync()
        {
            return await Task.FromResult(_students.Count);
        }

        public async Task<List<Student>> GetRecentStudentsAsync(int limit)
        {
            var recentStudents = _students.Values
                .OrderByDescending(s => s.EnrollmentDate)
                .Take(limit)
                .ToList();
            
            return await Task.FromResult(recentStudents);
        }
    }
}