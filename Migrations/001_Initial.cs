using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.baysideonline.BccMonday.Utilities;
using Rock.Plugin;

namespace com.baysideonline.BccMonday.Migrations
{
    [MigrationNumber(1, "1.11.4.3")]
    public class Initial : Migration
    {
        public override void Up()
        {
            Sql(@"
                CREATE TABLE [_com_baysideonline_BccMondayBoard] (
                    [Id] INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
                    [Guid] UNIQUEIDENTIFIER NOT NULL,
                    [CreatedDateTime] DATETIME NULL,
                    [ModifiedDateTime] DATETIME NULL,
                    [CreatedByPersonAliasId] INT NULL,
                    [ModifiedByPersonAliasId] INT NULL,
                    [ForeignKey] NVARCHAR(50) NULL,
                    [ForeignGuid] UNIQUEIDENTIFIER NULL,
                    [ForeignId] NVARCHAR(50) NULL,
                    [MondayBoardId] INT NOT NULL,
                    [MondayBoardName] VARCHAR(255) NULL,
                    [EmailMatchColumnId] VARCHAR(255) NULL,
                    [MondayStatusColumnId] VARCHAR(255) NULL,
                    [MondayStatusCompleteValue] VARCHAR(255) NULL,
                    [MondayStatusClosedValue] VARCHAR(255)
                );

                ALTER TABLE [dbo].[_com_baysideonline_BccMondayBoard]
                    ADD CONSTRAINT [FK_com_baysideonline_BccMondayBoard_PersonAlias_CreatedByPersonAliasId]
                    FOREIGN KEY([CreatedByPersonAliasId]) REFERENCES [dbo].[PersonAlias] (Id);

                ALTER TABLE [dbo].[_com_baysideonline_BccMondayBoard]
                    ADD CONSTRAINT [FK_com_baysideonline_BccMondayBoard_PersonAlias_ModifiedByPersonAliasId]
                    FOREIGN KEY([ModifiedByPersonAliasId]) REFERENCES [dbo].[PersonAlias] (Id);

                CREATE TABLE [dbo].[_com_baysideonline_BccMondayBoardDisplayColumn] (
	                [Id] INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	                [Guid] UNIQUEIDENTIFIER NOT NULL,
	                [CreatedDateTime] DATETIME NULL,
                    [CreatedByPersonAliasId] INT NULL,
	                [ModifiedDateTime] DATETIME NULL,
                    [ModifiedByPersonAliasId] INT NULL,
	                [ForeignKey] NVARCHAR(50) NULL,
	                [ForeignGuid] UNIQUEIDENTIFIER NULL,
	                [ForeignId] NVARCHAR(50) NULL,
	                [BccMondayBoardId] INT NOT NULL,
	                [MondayColumnId] VARCHAR(255) NOT NULL,
	                [MondayColumnTitle] VARCHAR(255) NOT NULL,
	                [MondayColumnType] VARCHAR(255) NULL,
                    [MondayColumnColor] VARCHAR(255) NULL
                );

                ALTER TABLE [dbo].[_com_baysideonline_BccMondayBoardDisplayColumn]
                    ADD CONSTRAINT [FK_com_baysideonline_BccMondayBoardDisplayColumn_PersonAlias_CreatedByPersonAliasId]
                    FOREIGN KEY([CreatedByPersonAliasId]) REFERENCES [dbo].[PersonAlias] (Id);

                ALTER TABLE [dbo].[_com_baysideonline_BccMondayBoardDisplayColumn]
                    ADD CONSTRAINT [FK_com_baysideonline_BccMondayBoardDisplayColumn_PersonAlias_ModifiedByPersonAliasId]
                    FOREIGN KEY([ModifiedByPersonAliasId]) REFERENCES [dbo].[PersonAlias] (Id);

                ALTER TABLE [dbo].[_com_baysideonline_BccMondayBoardDisplayColumn]
                    ADD CONSTRAINT [FK_com_baysideonline_BccMondayBoard_com_baysideonline_BccMondayBoardDisplayColumn]
                    FOREIGN KEY([BccMondayBoardId]) REFERENCES [dbo].[_com_baysideonline_BccMondayBoard] (Id);
            ");

            string textFieldTypeGuid = Rock.SystemGuid.FieldType.TEXT;

            RockMigrationHelper.AddGlobalAttribute(textFieldTypeGuid, null, null, "BccMondayApiKey",
                "The monday.com API key to use with the custom Bayside Rock integration", 1, null, Guids.MONDAY_API_KEY_GLOBAL_ATTRIB.ToString());
        }

        public override void Down()
        {
            Sql(@"
                ALTER TABLE [dbo].[_com_baysideonline_BccMondayBoard] DROP CONSTRAINT [FK_com_baysideonline_BccMondayBoard_PersonAlias_CreatedByPersonAliasId];
                ALTER TABLE [dbo].[_com_baysideonline_BccMondayBoard] DROP CONSTRAINT [FK_com_baysideonline_BccMondayBoard_PersonAlias_ModifiedByPersonAliasId];
                DROP TABLE [_com_baysideonline_BccMondayBoard];

                ALTER TABLE [dbo].[_com_baysideonline_BccMondayBoardDisplayColumn] DROP CONSTRAINT [FK_com_baysideonline_BccMondayBoardDisplayColumn_PersonAlias_CreatedByPersonAliasId];
                ALTER TABLE [dbo].[_com_baysideonline_BccMondayBoardDisplayColumn] DROP CONSTRAINT [FK_com_baysideonline_BccMondayBoardDisplayColumn_PersonAlias_ModifiedByPersonAliasId];
                ALTER TABLE [dbo].[_com_baysideonline_BccMondayBoardDisplayColumn] DROP CONSTRAINT [FK_com_baysideonline_BccMondayBoard_com_baysideonline_BccMondayBoardDisplayColumn];
                DROP TABLE [_com_baysideonline_BccMondayBoardDisplayColumn];
            ");

            RockMigrationHelper.DeleteAttribute(Guids.MONDAY_API_KEY_GLOBAL_ATTRIB.ToString());
        }
    }
}
