using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrations
{
    [Migration(20150622)]
    public class InitialDatabase : Migration
    {
        public override void Up()
        {
            Create.Table("Auction")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Title").AsString(80).Nullable()
                .WithColumn("Start").AsDateTime().NotNullable().Indexed("IX_auction_start")
                .WithColumn("End").AsDateTime().Nullable()
                .WithColumn("Type").AsInt32().NotNullable().WithDefaultValue(0)
                .WithColumn("State").AsInt32().NotNullable().WithDefaultValue(0).Indexed("IX_auction_state");
        }

        public override void Down()
        {
            Delete.Table("Auction");
        }
    }
}
