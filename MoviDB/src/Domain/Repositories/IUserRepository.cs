using MoviDB.Domain.Entities.User;

namespace MoviDB.Domain.Repositories;

public interface IUserQueryRepository
{
    Task<bool> ExistsByNameAsync(string name);
    Task<User> GetByNameAsync(string name);
}

public interface IUserCommandRepository
{
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
}