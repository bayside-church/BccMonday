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

    [TextField(
        "Name",
        Description = "The item's name.",
        Key = "Name",
        IsRequired = true,
        Order = 1
        )]
    [TextField(
        "Board Id",
        Description = "The item's Board.",
        Key = "BoardId",
        IsRequired = true,
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

            // We create an item with some values.
            // The values are mostly going to be the same except for the column values
            // the item will always have a board Id and a name.
            // the other column values will probably just be key-value pairs
            errorMessages = new List<string>();

            var name = GetAttributeValue(action, "Name");
            var boardId = GetAttributeValue(action, "BoardId");
            var requestor = GetAttributeValue(action, "Requestor");
            var columnValues = GetAttributeValue(action, "ColumnValues").AsDictionaryOrNull();

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
            throw new NotImplementedException();
        }
    }
}
