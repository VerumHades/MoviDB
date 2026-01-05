using MoviDB.domain.entities;

namespace MoviDB.domain.repositories;

public interface IGenreRepository
{
    Genre GetById(int id);
    Genre GetByName(string name);
    Genre Add(string name);  // returns the created Genre
}