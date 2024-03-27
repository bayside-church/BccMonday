using com.baysideonline.BccMonday.Utilities.Api;
using com.baysideonline.BccMonday.Utilities.Api.Config;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Workflow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.Workflows
{
    [ActionCategory("Bayside > Monday.com")]
    [Description("Create Monday.com Column")]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "Create Column")]

    [TextField(
        "Board Id",
        Description = "The Board Id to create the column.",
        Key = "BoardId",
        IsRequired = true,
        Order = 1
        )]
    [TextField(
        "Column Id",
        Description = "The custom Id of the new Column. If it is empty, then a random Id will be generated.",
        Key = "ColumnId",
        IsRequired = false,
        Order = 2
        )]
    [TextField(
        "Title",
        Description = "The new column's title.",
        Key = "Title",
        Order = 3
        )]
    [TextField(
        "Description",
        Description = "The new column's description.",
        Key = "Description",
        Order = 4
        )]
    [TextField(
        "Column Type",
        Description = "The new column's Column Type.",
        Key = "ColumnType",
        Order = 5,
        DefaultValue = "text"
        )]

    public class CreateColumn : ActionComponent
    {
        public override bool Execute(RockContext rockContext, WorkflowAction action, object entity, out List<string> errorMessages)
        {
            errorMessages = new List<string>();
            

            var boardId = GetAttributeValue(action, "BoardId");
            var title = GetAttributeValue(action, "Title");
            var columnId = GetAttributeValue(action, "ColumnId");
            var description = GetAttributeValue(action, "Description");
            var columnType = GetAttributeValue(action, "ColumnType");

            var api = new MondayApi();
            var options = new ColumnCreationOptions
            {
                Id = columnId,
                BoardId = long.Parse(boardId),
                Title = title,
                Description = description,
                ColumnType = columnType
            };

            var column = api.CreateColumn(options);

            if (column != null)
            {
                return true;
            }
            else
            {
                errorMessages.Add("Could not create the column in Monday.com");
            }

            return false;
        }
    }
}
