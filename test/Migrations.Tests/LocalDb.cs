using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlLocalDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Migrations.Tests
{

    public class LocalDb : IDisposable
    {
        private ISqlLocalDbApi _localDb;
        private bool _disposabled;
        private string _attachDbFileName;
        private string _dbName;
        private string _instanceName;

        public void ConfigureSettings()
        {
            _localDb = new SqlLocalDbApiWrapper();
            if (!_localDb.IsLocalDBInstalled())
            {
                throw new SqlLocalDbException("LocalDb não está instalado :(");
            }

            if (!Directory.Exists(@"C:\temp\")) Directory.CreateDirectory(@"C:\temp\");
            if (!Directory.Exists(@"C:\temp\localdb\")) Directory.CreateDirectory(@"C:\temp\localdb\");

            var id = Guid.NewGuid().ToString("N").Substring(0, 8);

            _attachDbFileName = @"C:\temp\localdb\" + id + ".mdf";
            _dbName = "db" + id;
            _instanceName = "v11.0";
        }

        protected string GetConnectionString()
        {
            return String.Format(@"Data Source=(LocalDb)\{2};AttachDBFileName={1};Initial Catalog={0};Integrated Security=True;", _dbName, _attachDbFileName, _instanceName);
        }

        protected string GetMasterConnectionString()
        {
            string connectionString = String.Format(@"Data Source=(LocalDb)\" + _instanceName + ";Initial Catalog=master;Integrated Security=True");
            return connectionString;
        }

        public void CreateDatabase()
        {
            using (var connection = new SqlConnection(GetMasterConnectionString()))
            {
                connection.Open();

                try
                {
                    var createCommand = String.Format("CREATE DATABASE [{0}] ON (NAME = N'{0}', FILENAME = '{1}')", _dbName, _attachDbFileName);
                    using (var command = new SqlCommand(createCommand, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public void DetachDatabase(string databaseName)
        {
            using (var connection = new SqlConnection(GetMasterConnectionString()))
            {
                connection.Open();

                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = String.Format("exec sp_detach_db '{0}'", databaseName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<string> GetDatabasesOnLocalDb()
        {
            var results = new List<string>();

            using (var connection = new SqlConnection(GetMasterConnectionString()))
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT name FROM master..sysdatabases where name not in ('master', 'model', 'msdb', 'tempdb')";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(reader.GetString(0));
                        }
                    }
                }
            }

            return results;
        }

        public void CleanUp()
        {
            foreach (var databasesName in GetDatabasesOnLocalDb())
            {
                try
                {
                    DetachDatabase(databasesName);
                }
                catch (Exception)
                {

                }
            }

            var files = Directory.GetFiles(@"C:\temp\localdb\");
            foreach (var file in files)
            {
                try { File.Delete(file); }
                catch { }
            }

            _disposabled = true;
        }

        public void Dispose()
        {
            if (!_disposabled) CleanUp();
        }
    }
}
