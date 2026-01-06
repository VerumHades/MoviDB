namespace MoviDB.Domain.Repositories;

public interface IUnitOfWork
{
    IMovieRepository Movies { get; }
    ISeriesRepository Series { get; }
    IGenreRepository Genres { get; }

    Task CommitAsync();
    Task RollbackAsync();
}
