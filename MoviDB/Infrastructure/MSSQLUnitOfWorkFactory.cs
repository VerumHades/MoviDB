using Microsoft.Data.SqlClient;
using MoviDB.Application.UnitOfWork;
using MoviDB.Infrastructure.Database;

namespace MoviDB.Infrastructure;

public class MSSQLUnitOfWorkFactory: IUnitOfWorkFactory
{
    private SqlConnectionFactory _connectionFactory;

    public MSSQLUnitOfWorkFactory(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IUnitOfWork> Create()
    {
        return new MSSQLUnitOfWork(_connectionFactory);
    }
}