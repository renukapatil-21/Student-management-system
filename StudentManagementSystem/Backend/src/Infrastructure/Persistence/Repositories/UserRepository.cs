using MongoDB.Driver;
using MongoDB.Bson;
using StudentManagementSystem.Domain.Entities;
using StudentManagementSystem.Domain.Interfaces;
using StudentManagementSystem.Domain.Enums;
using StudentManagementSystem.Application.Interfaces.Repositories;

namespace StudentManagementSystem.Infrastructure.Persistence.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(IMongoDatabase database) : base(database) { }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var filter = Builders<User>.Filter.Eq(x => x.Email, email);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role)
    {
        var filter = Builders<User>.Filter.Eq(x => x.Role, role);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        var filter = Builders<User>.Filter.Eq(x => x.Email, email);
        return await _collection.Find(filter).AnyAsync();
    }
}
}