namespace MoviDB.Application.UnitOfWork;

public interface IUnitOfWorkFactory
{
    Task<IUnitOfWork> Create();
}