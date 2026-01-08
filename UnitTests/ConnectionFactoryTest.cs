using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MoviDB.Infrastructure.Database;
using MoviDB.Infrastructure.Serialization;
using Xunit;

public class SqlConnectionFactoryTests
{
    private readonly SqlConnectionFactory _connectionFactory;

    public SqlConnectionFactoryTests()
    {
        var testConfigLoader = new DatabaseConnectionConfig()
        {
            Server = "DESKTOP-EMT3CHH\\SQLEXPRESS",
            Database = "movies2",
            UserId = "library_manager_login",
            Password = "LibraryManagerStrongPassword!123"
        };
        _connectionFactory = new SqlConnectionFactory(testConfigLoader);
    }

    [Fact]
    public async Task ConnectionFactory_UsesPooling()
    {
        const int iterations = 10;
        var firstOpenTimes = new long[iterations];

        for (int i = 0; i < iterations; i++)
        {
            var stopwatch = Stopwatch.StartNew();

            await using var connection = await _connectionFactory.CreateOpenConnectionAsync();

            stopwatch.Stop();
            firstOpenTimes[i] = stopwatch.ElapsedMilliseconds;
        }
        
        var first = firstOpenTimes[0];
        var restAverage = (long)(firstOpenTimes[1..].Sum() / (iterations - 1));

        Assert.True(restAverage < first, "Subsequent connections should be faster due to pooling");
    }
}