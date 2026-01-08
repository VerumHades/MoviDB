using MoviDB.Domain.Entities.Media;

namespace MoviDB.Domain.Repositories;

public interface IGenreQueryRepository
{
    Task<Genre?> GetByNameAsync(string name);
    Task<List<Genre>> GetAllAsync();
}

public interface IGenreCommandRepository
{
    Task<Genre> CreateAsync(string name);
}