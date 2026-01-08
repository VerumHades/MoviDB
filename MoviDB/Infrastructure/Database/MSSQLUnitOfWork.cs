using Microsoft.Data.SqlClient;
using MoviDB.Application.UnitOfWork;
using MoviDB.Domain.Repositories;
using MoviDB.Infrastructure.Database;

namespace MoviDB.Infrastructure;

/// <summary>
/// A unit-of-work implementation for SQL Server that coordinates transactions
/// and exposes repository interfaces without constructing concrete repositories internally.
/// </summary>
public sealed class MSSQLUnitOfWork : IUnitOfWork
{
    private readonly SqlConnection _connection;
    private readonly SqlTransaction _transaction;
    private bool _disposed;

    public IMovieCommandRepository Movies { get; }
    public ISeriesCommandRepository Series { get; }
    public IGenreCommandRepository Genres { get; }
    public IReviewCommandRepository Reviews { get; }

    /// <summary>
    /// Initializes a new instance of MSSQLUnitOfWork.
    /// Repositories are injected, and all operations share the same transaction.
    /// </summary>
    public MSSQLUnitOfWork(
        SqlConnection connection,
        SqlTransaction transaction,
        IMovieCommandRepository movies,
        ISeriesCommandRepository series,
        IGenreCommandRepository genres,
        IReviewCommandRepository reviews)
    {
        _connection = connection;
        _transaction = transaction ;

        Movies = movies;
        Series = series;
        Genres = genres;
        Reviews = reviews;
    }

    /// <summary>
    /// Commits the current transaction asynchronously.
    /// </summary>
    public Task CommitAsync() => _transaction.CommitAsync();

    /// <summary>
    /// Rolls back the current transaction asynchronously.
    /// </summary>
    public Task RollbackAsync() => _transaction.RollbackAsync();

    /// <summary>
    /// Disposes of the transaction and connection asynchronously.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        await _transaction.DisposeAsync();
        await _connection.DisposeAsync();
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _transaction.Dispose();
        _connection.Dispose();
    }
}
