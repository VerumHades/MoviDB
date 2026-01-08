namespace MoviDB.Application.Services;

using MoviDB.Application.UnitOfWork;
using MoviDB.Domain.Entities.Media;
using MoviDB.Domain.Exceptions;
using MoviDB.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


/// <summary>
/// Provides operations for managing genres.
/// </summary>
public class GenreService
{
    private readonly IGenreQueryRepository _genreQueryRepository;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public GenreService(
        IGenreQueryRepository genreQueryRepository,
        IUnitOfWorkFactory unitOfWorkFactory)
    {
        _genreQueryRepository = genreQueryRepository ?? throw new ArgumentNullException(nameof(genreQueryRepository));
        _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
    }

    /// <summary>
    /// Adds a new genre. Throws if a genre with the same name already exists.
    /// </summary>
    public async Task<Genre> AddGenreAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Genre name cannot be empty.", nameof(name));

        var existing = await _genreQueryRepository.GetByNameAsync(name);
        if (existing != null)
            throw new GenreAlreadyExistsException(name);

        return await _unitOfWorkFactory.ExecuteInTransactionAsync(async uow => await uow.Genres.CreateAsync(name));
    }

    /// <summary>
    /// Returns all genres ordered by name.
    /// </summary>
    public async Task<List<Genre>> ListGenresAsync()
    {
        // Ensure repository has a GetAllAsync method
        return await _genreQueryRepository.GetAllAsync();
    }
}
