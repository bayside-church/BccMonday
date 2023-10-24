using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rock.Plugin;

namespace com.baysideonline.BccMonday.Migrations
{
    [MigrationNumber(3, "1.11.4.3")]
    public class ApprovedValue : Migration
    {
        public override void Up()
        {
            Sql(@"
                ALTER TABLE [_com_baysideonline_BccMondayBoard]
                ADD [MondayStatusApprovedValue] VARCHAR(255)
                
                ALTER TABLE [_com_baysideonline_BccMondayBoard]
                ADD [ShowApprove] BIT NOT NULL DEFAULT 0
            ");
        }

        public override void Down()
        {
            Sql(@"
                ALTER TABLE [_com_baysideonline_BccMondayBoard]
                DROP COLUMN [MondayStatusApprovedValue]

                ALTER TABLE [_com_baysideonline_BccMondayBoard]
                DROP COLUMN [ShowApprove]
            ");
        }
    }
}
