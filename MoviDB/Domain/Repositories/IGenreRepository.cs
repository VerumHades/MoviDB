using MoviDB.Domain.Entities.Media;

namespace MoviDB.Domain.Repositories;

public interface IGenreQueryRepository
{
    Task<bool> ExistsByNameAsync(string name);
    Task<Genre> GetByNameAsync(string name);
}

public interface IGenreCommandRepository
{
    Task<Genre> CreateAsync(string name);
}