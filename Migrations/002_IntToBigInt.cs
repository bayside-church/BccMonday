using System;
using Rock.Plugin;

namespace com.baysideonline.BccMonday.Migrations
{
    [MigrationNumber(2, "1.11.4.3")]
    public class IntToBigInt : Migration
    {
        public override void Up()
        {
            Sql(@"
                ALTER TABLE [_com_baysideonline_BccMondayBoard] ALTER COLUMN [MondayBoardId] bigint
                ");

        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}