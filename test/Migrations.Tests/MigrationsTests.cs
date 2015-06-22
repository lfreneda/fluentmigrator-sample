using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrations.Tests
{
    [TestFixture]
    public class MigrationsTests : LocalDb
    {
        [SetUp]
        public void SetUp()
        {
            ConfigureSettings();
            CreateDatabase();
        }

        [Test]
        public void Run()
        {
            var migrator = new Migrator(GetConnectionString(), "sqlserver", typeof(InitialDatabase).Assembly);
            migrator.Migrate(runner => runner.MigrateUp());
        }

        [TearDown]
        public void TearDown()
        {
            Dispose();
        }
    }
}
