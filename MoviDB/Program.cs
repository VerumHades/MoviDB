using Microsoft.Extensions.DependencyInjection;
using MoviDB.Application.Services;
using MoviDB.Application.UnitOfWork;
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
    static void Main(string[] args)
    {
        var connectionFactory = new SqlConnectionFactory("DatabaseConfig.json");
        var unitOfWorkFactory = new MSSQLUnitOfWorkFactory(connectionFactory);
        var queryExecutor = new SqlServerAutocommitExecutor(connectionFactory.CreateOpenConnection());

        var seriesQueryRepository = new SqlSeriesQueryRepository(queryExecutor);
        var genreQueryRepository = new SqlGenreQueryRepository(queryExecutor);

        var seriesService = new SeriesService(seriesQueryRepository, genreQueryRepository, unitOfWorkFactory);

        var registry = new CommandRegistry();
        registry.RegisterCommand(new ImportSeriesCommand(seriesService));
        registry.RegisterCommand(new HelpCommand(registry));

        var console = new CommandConsole(registry);
        console.Run();
    }
}