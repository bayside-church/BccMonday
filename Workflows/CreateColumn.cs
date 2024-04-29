using com.baysideonline.BccMonday.Utilities.Api;
using com.baysideonline.BccMonday.Utilities.Api.Config;
using com.baysideonline.BccMonday.Utilities.Api.Schema;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Workflow;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text.RegularExpressions;

namespace com.baysideonline.BccMonday.Workflows
{
    [ActionCategory("Bayside > Monday.com")]
    [Description("Create Monday.com Column")]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "Create Column")]

    [WorkflowTextOrAttribute(
        "Board Id",
        "Attribute Value",
        Description = "The Board Id to create the column.",
        Key = "BoardId",
        IsRequired = true,
        FieldTypeClassNames = new string[] { "Rock.Field.Types.TextFieldType" },
        Order = 1
        )]
    [WorkflowTextOrAttribute(
        "Column Id",
        "Attribute Value",
        Description = "The custom Id of the new Column. If it is empty, then a random Id will be generated." +
        "\n- [1-20] characters in length (inclusive)\r\n" +
        "- Only lowercase letters (a-z) and underscores (_)\r\n" +
        "- Must be unique (no other column on the board can have the same ID)\r\n" +
        "- Can't reuse column IDs, even if the column has been deleted from the board\r\n" +
        "- Can't be null, blank, or an empty string   ",
        Key = "ColumnId",
        IsRequired = false,
        FieldTypeClassNames = new string[] { "Rock.Field.Types.TextFieldType" },
        Order = 2
        )]
    [WorkflowTextOrAttribute(
        "Title",
        "Attribute Value",
        Description = "The new column's title.",
        Key = "Title",
        FieldTypeClassNames = new string[] { "Rock.Field.Types.TextFieldType" },
        Order = 3
        )]
    [WorkflowTextOrAttribute(
        "Description",
        "Attribute Value",
        Description = "The new column's description.",
        Key = "Description",
        FieldTypeClassNames = new string[] { "Rock.Field.Types.TextFieldType" },
        Order = 4
        )]
    [WorkflowTextOrAttribute(
        "Column Type",
        "Attribute Value",
        Description = "The new column's Column Type.",
        Key = "ColumnType",
        DefaultValue = "text",
        FieldTypeClassNames = new string[] { "Rock.Field.Types.TextFieldType", "Rock.Field.Types.SelectSingleFieldType" },
        Order = 5
        )]

    public class CreateColumn : ActionComponent
    {
        public override bool Execute(RockContext rockContext, WorkflowAction action, object entity, out List<string> errorMessages)
        {
            errorMessages = new List<string>();

            var boardId = GetAttributeValue(action, "BoardId", true);
            var title = GetAttributeValue(action, "Title", true);
            var columnId = GetAttributeValue(action, "ColumnId", true);
            var description = GetAttributeValue(action, "Description", true);
            var columnType = GetAttributeValue(action, "ColumnType", true);

            var api = new MondayApi();
            var options = new ColumnCreationOptions
            {
                Id = columnId,
                BoardId = long.Parse(boardId),
                Title = title,
                Description = description,
                ColumnType = columnType
            };

            var isValid = IsValidColumn(columnId, boardId, errorMessages);

            if (!isValid)
            {
                errorMessages.Add("Could not create the column in Monday.com due to column Id constraints.");
                return false;
            }

            var column = api.CreateColumn(options);

            if (column != null)
            {
                action.AddLogEntry($"Monday.com Column ({columnId}) has been created for Board {boardId}");
                return true;
            }
            else
            {
                errorMessages.Add("Could not create the column in Monday.com");
                return false;
            }
        }

        public bool IsValidColumn(string columnId, string boardId, List<string> errorMessages)
        {
            var existingColumnIds = new MondayApi().GetBoard(long.Parse(boardId)).Columns.Select(c => c.Id).ToList();
            var regex = new Regex("^[a-z_]+$");
            var hasBadCharacters = !regex.IsMatch(columnId);
            var isExistingColumnId = existingColumnIds.Contains(columnId);
            var isIncorrectLength = columnId.Length < 1 || columnId.Length > 20;
            var isBlankOrEmpty = columnId.IsNullOrWhiteSpace();

            if (hasBadCharacters)
            {
                errorMessages.Add($"The provided Column Id ({columnId}) can only contain lowercase letters and underscores.");
            }

            if (isExistingColumnId)
            {
                errorMessages.Add($"The provided Column Id ({columnId}) already exists for the Monday.com Board ({ boardId })");
            }

            if (isIncorrectLength)
            {
                errorMessages.Add($"The provided Column Id ({columnId}) can only be between 1-20 characters (inclusive). Given length is {columnId.Length}");
            }

            if (isBlankOrEmpty)
            {
                errorMessages.Add("The provided Column Id is blank or empty.");
            }

            if (hasBadCharacters || isExistingColumnId || isIncorrectLength || isBlankOrEmpty)
            {
                return false;
            }

            return true;
        }
    }
}
