using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrations
{
    [Migration(20150623)]
    public class RenameColumn : Migration
    {
        public override void Up()
        {
            Rename.Column("Title").OnTable("Auction").To("Description");
        }

        public override void Down()
        {
            Rename.Column("Description").OnTable("Auction").To("Title");
        }
    }
}
