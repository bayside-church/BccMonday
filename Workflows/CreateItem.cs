using Rock.Data;
using Rock.Model;
using Rock.Workflow;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rock.Attribute;
using com.baysideonline.BccMonday.Utilities.Api;
using Rock;
using Newtonsoft.Json;

namespace com.baysideonline.BccMonday.Workflows
{
    [ActionCategory("Bayside > Monday.com")]
    [Description("Create Monday.com Item")]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "Create Item")]

    [WorkflowTextOrAttribute(
        "Name",
        "Attribute Value",
        Description = "The item's name or an attribute that contains the item's name.",
        Key = "Name",
        IsRequired = true,
        FieldTypeClassNames = new string[] { "Rock.Field.Types.TextFieldType" },
        Order = 1
        )]
    [WorkflowTextOrAttribute(
        "Board Id",
        "Attribute Value",
        Description = "The item's Board Id or an attribute that contains the Board Id.",
        Key = "BoardId",
        IsRequired = true,
        FieldTypeClassNames = new string[] { "Rock.Field.Types.TextFieldType" },
        Order = 2
        )]
    [WorkflowTextOrAttribute(
        "Group Id",
        "Attribute Value",
        Description = "The item's Group Id or an attribute that contains the Group Id.",
        Key = "GroupId",
        IsRequired = false,
        FieldTypeClassNames = new string[] { "Rock.Field.Types.TextFieldType" },
        Order = 3
        )]
    [KeyValueListField(
        "Column Values",
        Description = "The column values of the new Item. The keys must all be lowercase.",
        Key = "ColumnValues",
        Order = 4
        )]
    [WorkflowAttribute(
        "Item Id Attribute",
        "An optional attribute to set to the Id of the Monday.com Item.",
        false,
        "",
        "",
        Key = "ItemIdResultAttribute",
        Order = 5
        )]
    [WorkflowAttribute(
        "Item Attribute",
        "An optional attribute to set to the JSON value of the Monday.com Item.",
        false,
        "",
        "",
        Key = "ItemResultAttribute",
        Order = 6
        )]
    [BooleanField(
        "Continue on Error",
        "Should processing continue even if a Monday.com API error occurrs?",
        false,
        "",
        Key = "ContinueOnError",
        Order = 7
        )]


    public class CreateItem : ActionComponent
    {
        public override bool Execute(RockContext rockContext, WorkflowAction action, object entity, out List<string> errorMessages)
        {
            errorMessages = new List<string>();

            var mergeFields = GetMergeFields(action);
            var name = GetAttributeValue(action, "Name", true).ResolveMergeFields(mergeFields);
            var boardId = GetAttributeValue(action, "BoardId", true).ResolveMergeFields(mergeFields);
            var groupId = GetAttributeValue(action, "GroupId", true).ResolveMergeFields(mergeFields);
            var requestor = GetAttributeValue(action, "Requestor");
            var columnValuesDict = GetAttributeValue(action, "ColumnValues").AsDictionaryOrNull();

            var columnValues = columnValuesDict.Select(c => new { c.Key, Value = c.Value.ResolveMergeFields(mergeFields) })
                .Where(c => c.Value.IsNotNullOrWhiteSpace())
                .ToDictionary(k => k.Key, k => k.Value);

            var serializedCV = JsonConvert.SerializeObject(columnValues);

            var options = new Utilities.Api.Config.ItemCreationOptions
            {
                Name = name,
                BoardId = long.Parse(boardId),
                GroupId = groupId,
                ColumnValues = columnValues,
                CreateLabelsIfMissing = false
            };

            var api = new MondayApi();

            var item = api.CreateItem(options);

            if (item != null)
            {
                action.AddLogEntry($"Monday.com Item({ item.Id }) has been created.");
                var itemId = SetWorkflowAttributeValue(action, "ItemIdResultAttribute", item.Id);
                var itemResult = SetWorkflowAttributeValue(action, "ItemResultAttribute", JsonConvert.SerializeObject(item));

                if (itemId != null)
                {
                   action.AddLogEntry($"Set {itemId.Name} attribute to {item.Id}");
                }

                if (itemResult != null)
                {
                    action.AddLogEntry($"Set {itemResult.Name} attribute to {JsonConvert.SerializeObject(item)}");
                }
                return true;
            }
            else
            {
                var error = "Could not create Monday.com Item";
                action.AddLogEntry(error);
                errorMessages.Add(error);

                if (!GetAttributeValue(action, "ContinueOnError").AsBoolean())
                {
                    return false;
                }

                return true;

            }
        }
    }
}
