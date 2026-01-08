namespace MoviDB.Application.UnitOfWork;

public interface IUnitOfWorkFactory
{
    Task<T> ExecuteInTransactionAsync<T>(Func<IUnitOfWork, Task<T>> work);
    Task ExecuteInTransactionAsync(Func<IUnitOfWork, Task> work);
}