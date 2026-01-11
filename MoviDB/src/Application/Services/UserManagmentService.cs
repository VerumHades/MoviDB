using MoviDB.Application.UnitOfWork;
using MoviDB.Domain.DTOs;
using MoviDB.Domain.Entities.User;
using MoviDB.Domain.Repositories;

namespace MoviDB.Application.Services;

public class UserManagmentService
{
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public UserManagmentService(IUserQueryRepository userQueryRepository, IUnitOfWorkFactory unitOfWorkFactory)
    {
        _userQueryRepository = userQueryRepository;
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<(UserProjection[], UserCursor)> GetNextBatchOfAllAsync(int batchSize, UserCursor? cursor = null)
    {
        return await _userQueryRepository.GetNextBatchOfAllAsync(batchSize, cursor);
    }

    public async Task CreateUser(string username, string password)
    {
        await _unitOfWorkFactory.ExecuteInTransactionAsync(async uow =>
        {
            await uow.Users.CreateAsync(new User(username, password, UserRole.Normal));
        });
    }

    public async Task DeleteUser(string username)
    {
        User user = await _userQueryRepository.GetByNameAsync(username);
        
        await _unitOfWorkFactory.ExecuteInTransactionAsync(async uow =>
        {
            await uow.Users.DeleteAsync(user.Id);
        });
    }
}