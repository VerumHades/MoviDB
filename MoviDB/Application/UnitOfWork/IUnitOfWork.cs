using MoviDB.Domain.Repositories;

namespace MoviDB.Application.UnitOfWork;

public interface IUnitOfWork: IAsyncDisposable, IDisposable
{
    IMovieCommandRepository Movies { get; }
    ISeriesCommandRepository Series { get; }
    IGenreCommandRepository Genres { get; }
    
    IReviewCommandRepository Reviews { get; }

    Task CommitAsync();
    Task RollbackAsync();
}
