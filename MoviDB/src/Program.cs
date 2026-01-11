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
using MoviDB.Presentation.CLI.Commands.User;

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

        var seriesQueryRepository = new SqlSeriesQueryRepository(queryExecutor);
        var movieQueryRepository = new SqlMovieQueryRepository(queryExecutor);
        var genreQueryRepository = new SqlGenreQueryRepository(queryExecutor);
        var userQueryRepository = new SqlUserQueryRepository(queryExecutor);

        var seriesService = new SeriesService(seriesQueryRepository, genreQueryRepository, unitOfWorkFactory);
        var movieService = new MovieService(movieQueryRepository, genreQueryRepository, unitOfWorkFactory);
        var genreService = new GenreService(genreQueryRepository, unitOfWorkFactory);
        var userManagmentService = new UserManagmentService(userQueryRepository, unitOfWorkFactory);
        var userService = new UserService(unitOfWorkFactory, userQueryRepository, movieQueryRepository,
            seriesQueryRepository);

        var movieReviewService = new MovieReviewService(movieQueryRepository, userQueryRepository, unitOfWorkFactory);
        var seriesReviewService =
            new SeriesReviewService(seriesQueryRepository, userQueryRepository, unitOfWorkFactory);
        
        var registry = new CommandRegistry();
        foreach (var command in
             new List<ICommand> {
                 new ImportSeriesCommand(seriesService),
                 new HelpCommand(registry),
                 new RegisterMovieCommand(movieService),
                 
                 MovieCommands.CreateListMoviesCommand(movieService),
                 SeriesCommands.ListSeriesCommand(seriesService),
                 
                 new CreateGenreCommand(genreService),
                 new ListGenresCommand(genreService),
                 new UpdateMovieCommand(movieService),
                 new DeleteMovieCommand(movieService),
                 
                 UserManagmentCommands.CreateListUsersCommand(userManagmentService),
                 UserManagmentCommands.CreateUserCommand(userManagmentService),
                 UserManagmentCommands.DeleteUserCommand(userManagmentService),
                 
                 UserCommands.AddMovieToLibraryCommand(userService),
                 UserCommands.AddSeriesToLibraryCommand(userService),
                 UserCommands.MarkMovieWatchedCommand(userService),
                 UserCommands.MarkSeriesWatchedCommand(userService),
                 
                 // Movie reviews
                 MovieReviewCommands.AddMovieReviewCommand(movieReviewService),
                 MovieReviewCommands.UpdateMovieReviewCommand(movieReviewService),
                 MovieReviewCommands.RemoveMovieReviewCommand(movieReviewService),

                 // Series reviews
                 SeriesReviewCommands.AddSeriesReviewCommand(seriesReviewService),
                 SeriesReviewCommands.UpdateSeriesReviewCommand(seriesReviewService),
                 SeriesReviewCommands.RemoveSeriesReviewCommand(seriesReviewService),
             })
        {
            registry.RegisterCommand(command);
        }

        var console = new CommandConsole(registry, Console.In, Console.Out);
        console.Run();

        return 0;
    }
}