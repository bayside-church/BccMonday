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
    [PersonField(
        "Requestor",
        Description = "The person requesting the item be created.",
        Key = "Requestor",
        IsRequired = true,
        Order = 3
        )]
    [KeyValueListField(
        "Column Values",
        Description = "The column values of the new Item. The keys must all be lowercase.",
        Key = "ColumnValues",
        Order = 4
        )]

    public class CreateItem : ActionComponent
    {
        public override bool Execute(RockContext rockContext, WorkflowAction action, object entity, out List<string> errorMessages)
        {
            errorMessages = new List<string>();

            var mergeFields = GetMergeFields(action);
            var name = GetAttributeValue(action, "Name").ResolveMergeFields(mergeFields);
            var boardId = GetAttributeValue(action, "BoardId").ResolveMergeFields(mergeFields);
            var requestor = GetAttributeValue(action, "Requestor");
            var columnValuesDict = GetAttributeValue(action, "ColumnValues").AsDictionaryOrNull();

            var columnValues = columnValuesDict.Select(c => new { c.Key, Value = c.Value.ResolveMergeFields(mergeFields) })
                .ToDictionary(k => k.Key, k => k.Value);

            var serializedCV = JsonConvert.SerializeObject(columnValues);

            var options = new Utilities.Api.Config.ItemCreationOptions
            {
                Name = name,
                BoardId = long.Parse(boardId),
                ColumnValues = columnValues,
                CreateLabelsIfMissing = false
            };

            var api = new MondayApi();

            var item = api.CreateItem(options);
            return true;
        }
    }
}
