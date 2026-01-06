namespace MoviDB.Domain.Repositories;

public interface IUnitOfWorkFactory
{
    IUnitOfWork Create();
}