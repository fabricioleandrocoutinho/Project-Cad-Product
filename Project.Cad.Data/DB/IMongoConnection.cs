using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Cad.Data.DB
{
    public interface IMongoConnection
    {
        IMongoDatabase Db { get; }
        MongoClientSettings Settings { get; }
        IMongoCollection<T> GetCollection<T>(string collectionName, Action<IMongoCollection<T>> createCollectionIndexes = null);
        Task<IMongoCollection<T>> GetCollectionAsync<T>(string collectionName, Func<IMongoCollection<T>, Task> createCollectionIndexes = null);
        FilterDefinitionBuilder<T> GetFilter<T>();
    }
}