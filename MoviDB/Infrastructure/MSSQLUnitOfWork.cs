using Microsoft.Data.SqlClient;
using MoviDB.Application.UnitOfWork;
using MoviDB.Domain.Repositories;
using MoviDB.Infrastructure.Repositories;
using MoviDB.Infrastructure.Database;

namespace MoviDB.Infrastructure;

public sealed class MSSQLUnitOfWork :
    IUnitOfWork,
    IAsyncDisposable
{
    private readonly SqlConnection connection;
    private readonly SqlTransaction transaction;
    private readonly ISqlExecutor sqlExecutor;
    private bool disposed;

    public IMovieCommandRepository Movies { get; }
    public ISeriesCommandRepository Series { get; }
    public IGenreCommandRepository Genres { get; }
    
    public IReviewCommandRepository Reviews { get; }

    public MSSQLUnitOfWork(SqlConnectionFactory connectionFactory)
    {
        connection = connectionFactory.CreateOpenConnection();
        transaction = connection.BeginTransaction();

        sqlExecutor = new SqlServerTransactionalExecutor(connection, transaction);

        Movies = new SqlMovieCommandRepository(sqlExecutor);
        Series = new SqlSeriesCommandRepository(sqlExecutor);
        Genres = new SqlGenreCommandRepository(sqlExecutor);
        Reviews = new SqlReviewCommandRepository(sqlExecutor);
    }

    public Task CommitAsync()
    {
        transaction.Commit();
        return Task.CompletedTask;
    }

    public Task RollbackAsync()
    {
        transaction.Rollback();
        return Task.CompletedTask;
    }

    public Task<T> ExecuteInTransaction<T>(Func<ISqlExecutor, Task<T>> executor)
    {
        return executor(sqlExecutor);
    }

    public async ValueTask DisposeAsync()
    {
        await connection.DisposeAsync();
        await transaction.DisposeAsync();
    }

    public void Dispose()
    {
        connection.Dispose();
        transaction.Dispose();
    }
}