using MoviDB.Domain.Repositories;

namespace MoviDB.Application.UnitOfWork;

public interface IUnitOfWork
{
    IMovieRepository Movies { get; }
    ISeriesRepository Series { get; }
    IGenreRepository Genres { get; }

    Task CommitAsync();
    Task RollbackAsync();
}
