using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrations
{
    [Migration(20150624)]
    public class CreateUsersTable : Migration
    {
        public override void Up()
        {
            Create.Table("Users")
                .WithColumn("UserId").AsInt32().PrimaryKey().Identity()
                .WithColumn("Name").AsString()
                .WithColumn("Email").AsAnsiString(128).NotNullable();

            Create.Index().OnTable("Users").OnColumn("Email").Unique();
        }

        public override void Down()
        {
            Delete.Table("Users");
        }
    }
}
