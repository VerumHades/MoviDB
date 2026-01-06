namespace MoviDB.Domain.Repositories;

public interface IMediaExistenceChecker
{
    Task<bool> MediaExistsAsync(int id);
}