using System;
using Rock.Plugin;

namespace com.baysideonline.BccMonday.Migrations
{
    [MigrationNumber(4, "1.11.4.3")]
    public class FixNullColumn : Migration
    {
        public override void Up()
        {
            Sql(@"
                ALTER TABLE [_com_baysideonline_BccMondayBoard] ALTER COLUMN [MondayBoardId] bigint NOT NULL
                ");

        }

        public override void Down()
        {
            Sql(@"
                ALTER TABLE [_com_baysideonline_BccMondayBoard] ALTER COLUMN [MondayBoardId] bigint NULL
                ");
        }
    }
}