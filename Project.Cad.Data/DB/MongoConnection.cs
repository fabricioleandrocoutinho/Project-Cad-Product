using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;


namespace Project.Cad.Data.DB
{

    public class MongoConnection : IMongoConnection
    {
        private static readonly object SyncDb = new();
        private static readonly object SyncSettings = new();

        private readonly MongoDbConfig config;
        private readonly IMongoClient _client;

        private IMongoDatabase _mongoDatabase;
        private MongoClientSettings _settings;

        public MongoConnection(MongoDbConfig config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            ThrowIfValuesInvalid(config);
            _settings = GetClientSettings();
            _client = new MongoClient(_settings);
        }

        public MongoConnection() : this(MongoDbConfig.GetFromEnvs())
        {
        }

        public MongoConnection(MongoDbConfig config, IMongoClient client) : this(config)
        {
            _client = client;
        }

        private static void ThrowIfValuesInvalid(MongoDbConfig config)
        {
            if (string.IsNullOrEmpty(config.DatabaseName))
            {
                throw new InvalidOperationException($"\"{nameof(config.DatabaseName)}\" is null or empty");
            }

            if (string.IsNullOrEmpty(config.ConnectionString))
            {
                throw new InvalidOperationException($"\"{nameof(config.ConnectionString)}\" is null or empty");
            }
        }

        public MongoClientSettings Settings
        {
            get
            {
                if (_settings != null) return _settings;

                lock (SyncSettings)
                {
                    _settings ??= GetClientSettings();
                }

                return _settings;
            }
        }

        public IMongoDatabase Db
        {
            get
            {
                if (_mongoDatabase != null) return _mongoDatabase;

                lock (SyncDb)
                {
                    _mongoDatabase ??= Connect();
                }

                return _mongoDatabase;
            }
        }

        public FilterDefinitionBuilder<T> GetFilter<T>()
        {
            return Builders<T>.Filter;
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName, Action<IMongoCollection<T>> createCollectionIndexes = null)
        {
            var collection = Db.GetCollection<T>(collectionName);
            createCollectionIndexes?.Invoke(collection);
            return collection;
        }

        public async Task<IMongoCollection<T>> GetCollectionAsync<T>(string collectionName, Func<IMongoCollection<T>, Task> createCollectionIndexes = null)
        {
            var collection = Db.GetCollection<T>(collectionName);
            await createCollectionIndexes?.Invoke(collection);
            return collection;
        }

        private IMongoDatabase Connect()
        {
            return _client.GetDatabase(config.DatabaseName);
        }

        private MongoClientSettings GetClientSettings()
        {

            var mongoUrl = new MongoUrl(config.ConnectionString.Trim());
            var settings = MongoClientSettings.FromUrl(mongoUrl);

            if (!string.IsNullOrEmpty(config.Certificate?.Path) &&
                !string.IsNullOrEmpty(config.Certificate?.Password))
            {
                settings.UseTls = true;
                settings.AllowInsecureTls = true;

                var cert = new X509Certificate2(config.Certificate.Path, config.Certificate.Password);

                settings.SslSettings = new SslSettings()
                {
                    ClientCertificates = new[] { cert }
                };
            }
            else if (!string.IsNullOrEmpty(config.Certificate?.Subject))
            {
                using var certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                certStore.Open(OpenFlags.ReadOnly);

                var certCollection = certStore.Certificates.Find(X509FindType.FindBySubjectName,
                    config.Certificate.Subject,
                                                                 validOnly: false);

                var certificate = certCollection.OfType<X509Certificate2>()
                                                .FirstOrDefault();

                if (certificate == null)
                    throw new InvalidOperationException($"Was not possible find certificate with subject name {config.Certificate.Subject}");

                settings.UseTls = true;
                settings.AllowInsecureTls = true;
                settings.SslSettings = new SslSettings()
                {
                    ClientCertificates = new[] { certificate }
                };
            }

            return settings;
        }

        private static string GetMd5HashFromFile(string fileName)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(fileName);
            return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
        }
    }
}