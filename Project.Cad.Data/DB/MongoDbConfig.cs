using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Cad.Data.DB
{
    public class MongoDbConfig
    {
        public MongoDbConfig()
        {
            Certificate = new MongoDbCertificateConfig();
        }

        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
        public MongoDbCertificateConfig Certificate { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="MongoDbConfig"/> loading each parameter 
        /// from default environment variables names
        /// 
        /// See each parameter to check the name of the env variable that it's loaded from.
        /// 
        /// </summary>
        /// <param name="certificatePassword">Env name: <b>DATABASE_CERTIFICATE_PASSWORD</b></param>
        /// <param name="certificatePath">Env name: <b>DATABASE_CERTIFICATE_PATH</b></param>
        /// <param name="subjectName">Env name: <b>DATABASE_CERTIFICATE_SUBJECTNAME</b></param>
        /// <param name="connectionString">Env name: <b>DATABASE_CONNECTIONSTRING</b></param>
        /// <param name="databaseName">Env name: <b>DATABASE_NAME</b></param>
        /// <returns></returns>
        public static MongoDbConfig GetFromEnvs(
            string certificatePassword = null,
            string certificatePath = null,
            string subjectName = null,
            string connectionString = null,
            string databaseName = null)
        {
            return new MongoDbConfig
            {
                Certificate = new MongoDbCertificateConfig
                {
                    Password = certificatePassword ?? Environment.GetEnvironmentVariable("DATABASE_CERTIFICATE_PASSWORD"),
                    Path = certificatePath ?? Environment.GetEnvironmentVariable("DATABASE_CERTIFICATE_PATH"),
                    Subject = subjectName ?? Environment.GetEnvironmentVariable("DATABASE_CERTIFICATE_SUBJECTNAME")
                },
                ConnectionString = connectionString ?? Environment.GetEnvironmentVariable("DATABASE_CONNECTIONSTRING"),
                DatabaseName = databaseName ?? Environment.GetEnvironmentVariable("DATABASE_NAME")
            };
        }
    }
}