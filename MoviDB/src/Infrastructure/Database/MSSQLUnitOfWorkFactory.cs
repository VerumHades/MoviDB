using Microsoft.Data.SqlClient;
using MoviDB.Application.UnitOfWork;
using MoviDB.Infrastructure.Database;
using MoviDB.Infrastructure.Repositories;

namespace MoviDB.Infrastructure;

public class MSSQLUnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly SqlConnectionFactory _connectionFactory;

    public MSSQLUnitOfWorkFactory(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<T> ExecuteInTransactionAsync<T>(Func<IUnitOfWork, Task<T>> work)
    {
        await using var uow = await CreateAsync();
        
        try
        {
            var value = await work(uow);
            await uow.CommitAsync();
            return value;
        }
        catch
        {
            await uow.RollbackAsync();
            throw;
        }
    }
    
    public async Task ExecuteInTransactionAsync(Func<IUnitOfWork, Task> work)
    {
        await using var uow = await CreateAsync(); // your async UoW factory

        try
        {
            await work(uow);       // call the callback
            await uow.CommitAsync(); // commit transaction
        }
        catch
        {
            await uow.RollbackAsync(); // rollback if exception occurs
            throw;
        }
    }

    

    private async Task<IUnitOfWork> CreateAsync()
    {
        var connection = await _connectionFactory.CreateOpenConnectionAsync();

        if (await connection.BeginTransactionAsync() is not SqlTransaction sqlTransaction)
            throw new InvalidOperationException("Expected SqlTransaction.");
        
        var sqlExecutor = new SqlServerTransactionalExecutor(connection, sqlTransaction);

        return new MSSQLUnitOfWork(
            connection,
            sqlTransaction,
            new SqlMovieCommandRepository(sqlExecutor),
            new SqlSeriesCommandRepository(sqlExecutor),
            new SqlGenreCommandRepository(sqlExecutor),
            new SqlReviewCommandRepository(sqlExecutor),
            new SqlUserCommandRepository(sqlExecutor),
            new SqlUserLibraryCommandRepository(sqlExecutor)
        );
    }
}