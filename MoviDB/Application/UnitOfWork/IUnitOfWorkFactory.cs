namespace MoviDB.Application.UnitOfWork;

public interface IUnitOfWorkFactory
{
    IUnitOfWork Create();
}