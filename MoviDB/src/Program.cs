using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using MoviDB.Application.Services;
using MoviDB.Application.UnitOfWork;
using MoviDB.Domain.Entities.Media;
using MoviDB.Domain.Repositories;
using MoviDB.Infrastructure;
using MoviDB.Infrastructure.Database;
using MoviDB.Infrastructure.Repositories;
using MoviDB.Infrastructure.Serialization;
using MoviDB.Presentation.CLI;
using MoviDB.Presentation.CLI.Commands;

namespace MoviDB;

class Program
{
    static int Main(string[] args)
    {
        DatabaseConnectionConfig? config = null;
        try
        {
            config = new JsonFileConfigLoader("DatabaseConfig.json").LoadConfiguration();
        }
        catch
        {
            Console.WriteLine("Failed to load configuration file? Maybe 'DatabaseConfig.json' is missing beside the executable?");
            return 1;
        }

        SqlConnectionFactory? connectionFactory;
        
        try
        {
            connectionFactory = new SqlConnectionFactory(config);
            using var initialConnection = connectionFactory.CreateOpenConnection();
        }
        catch(Exception ex)
        {
            Console.WriteLine("Failed to open initial connection: {0}", ex.Message);
            return 2;
        }
        
        var unitOfWorkFactory = new MSSQLUnitOfWorkFactory(connectionFactory);
        
        var queryExecutor = new SqlServerAutocommitExecutor(connectionFactory);
        queryExecutor.QueryAsync<string>("SELECT * FROM dbo.vw_movie", new Dictionary<string, object>(), record => null).GetAwaiter().GetResult();

        var seriesQueryRepository = new SqlSeriesQueryRepository(queryExecutor);
        var movieQueryRepository = new SqlMovieQueryRepository(queryExecutor);
        var genreQueryRepository = new SqlGenreQueryRepository(queryExecutor);

        var seriesService = new SeriesService(seriesQueryRepository, genreQueryRepository, unitOfWorkFactory);
        var movieService = new MovieService(movieQueryRepository, genreQueryRepository, unitOfWorkFactory);
        var genreService = new GenreService(genreQueryRepository, unitOfWorkFactory);
        
        var registry = new CommandRegistry();
        foreach (var command in
             new List<ICommand> {
                 new ImportSeriesCommand(seriesService),
                 new HelpCommand(registry),
                 new RegisterMovieCommand(movieService),
                 new ListMoviesCommand(movieService),
                 new CreateGenreCommand(genreService),
                 new ListGenresCommand(genreService),
                 new UpdateMovieCommand(movieService),
                 new DeleteMovieCommand(movieService)
             })
        {
            registry.RegisterCommand(command);
        }

        var console = new CommandConsole(registry, Console.In, Console.Out);
        console.Run();

        return 0;
    }
}