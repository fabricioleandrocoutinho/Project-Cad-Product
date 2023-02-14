using Mongo2Go;
using MongoDB.Driver;
using Project.Cad.Data.DB;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace Project.Cad.NUnitTest.ReaderService;

[ExcludeFromCodeCoverage]
public class MongoConnectionTest : IMongoConnection, IDisposable
{
    public IMongoDatabase Db => _db;

    public MongoClientSettings Settings => throw new NotImplementedException();

    private readonly IMongoDatabase _db;

    private static readonly MongoDbRunner _runner = MongoDbRunner.StartForDebugging(binariesSearchDirectory: GetPath());

    public MongoConnectionTest()
    {
        var client = new MongoClient(_runner.ConnectionString);
        _db = client.GetDatabase("IntegrationTest");
    }

    private static string GetPath()
    {
        var filePath = Path.GetDirectoryName(typeof(MongoConnectionTest).Assembly.Location).Split("\\Project.Data.NUnitTest");
        return Path.Combine(filePath[0], "NugetPackage");
    }

    public IMongoCollection<T> GetCollection<T>()
    {
        return _db.GetCollection<T>("TestCollection");
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName, Action<IMongoCollection<T>> createCollectionIndexes = null)
    {
        return _db.GetCollection<T>("TestCollection");
    }

    public Task<IMongoCollection<T>> GetCollectionAsync<T>(string collectionName, Func<IMongoCollection<T>, Task> createCollectionIndexes = null)
    {
        return Task.FromResult(_db.GetCollection<T>("TestCollection"));
    }

    public FilterDefinitionBuilder<T> GetFilter<T>()
    {
        return Builders<T>.Filter;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);

    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _runner.Dispose();
        }
    }
}
