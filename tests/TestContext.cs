using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace tests;

/// <summary>
/// Creates a Context with all Abstractions, the TestDbContext etc. to test the features
/// </summary>
public sealed class TestContext : IDisposable
{
    private const string ConnectionString = "Filename=:memory:";
    private readonly IServiceScope _scope;
    private readonly SqliteConnection _connection;
    public IServiceProvider Provider { get; }

    public TestContext()
    {
        _connection = new SqliteConnection(ConnectionString);
        _connection.Open();

        var collection = new ServiceCollection();
        collection.RegisterEfAbstraction<TestDbContext>(options => options.UseSqlite(_connection));

        Provider = collection.BuildServiceProvider();
        _scope = Provider.CreateScope();
        var evaluatorDbContext = GetDbContext();
        evaluatorDbContext.Database.EnsureCreated();
    }

    public TestDbContext GetDbContext() => _scope.ServiceProvider.GetRequiredService<TestDbContext>();
    public object GetService(Type t) => _scope.ServiceProvider.GetService(t);

    public void Dispose()
    {
        _scope?.Dispose();
        _connection?.Dispose();
    }
}