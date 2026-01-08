namespace MoviDB.Infrastructure.Repositories;

public abstract class Repository
{
    protected readonly ISqlExecutor _sqlExecutor;

    public Repository(ISqlExecutor sqlExecutor)
    {
        _sqlExecutor = sqlExecutor;
    }
}