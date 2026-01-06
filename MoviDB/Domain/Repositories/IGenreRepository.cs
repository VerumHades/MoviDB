using MoviDB.Domain.Entities.Media;

namespace MoviDB.Domain.Repositories;

public interface IGenreRepository
{
    Task<bool> ExistsByNameAsync(string name);
    Task<Genre> GetByNameASync(string name);
    Task<Genre> CreateAsync(string name);
}