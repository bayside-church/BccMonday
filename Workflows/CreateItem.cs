﻿using Rock.Data;
using Rock.Model;
using Rock.Workflow;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel;
using System.Linq;
using Rock.Attribute;
using Rock;
using Newtonsoft.Json;
using com.baysideonline.BccMonday.Utilities.Api;
using com.baysideonline.BccMonday.Utilities.Api.Config;
using com.baysideonline.BccMonday.Models;

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
            var boardId = long.Parse(GetAttributeValue(action, "BoardId", true).ResolveMergeFields(mergeFields));
            var groupId = GetAttributeValue(action, "GroupId", true).ResolveMergeFields(mergeFields);
            var columnValuesDict = GetAttributeValue(action, "ColumnValues").AsDictionaryOrNull();

            var columnValues = columnValuesDict.Select(c => new { c.Key, Value = c.Value.ResolveMergeFields(mergeFields) })
                .Where(c => c.Value.IsNotNullOrWhiteSpace())
                .ToDictionary(k => k.Key, k => k.Value);

            var missingRequester = "Missing requester values";

            var bccMondayBoard = new BccMondayBoardService(new RockContext()).Queryable().Where(b => b.MondayBoardId == boardId).FirstOrDefault();
            bccMondayBoard.LoadAttributes();
            var requesterColumn = bccMondayBoard.GetAttributeValue("RequesterAliasId");

            var defaultKVPair = default(KeyValuePair<string, string>);
            var requesterAliasCV = columnValues.FirstOrDefault(c => c.Key == requesterColumn);
            if (!requesterAliasCV.Equals(defaultKVPair))
            {
                var requesterValues = requesterAliasCV.Value.Split(',').Select(v => v.Trim()).ToList();

                if (requesterValues.Count < 1)
                {
                    action.AddLogEntry(missingRequester);
                    errorMessages.Add(missingRequester);
                    throw new System.Exception("No requester value found");
                }
                var requesterFormat = string.Join("|", requesterValues);
                requesterFormat = $"|{requesterFormat}|";
                columnValues.AddOrReplace(requesterColumn, requesterFormat);
            }

            var options = new ItemCreationOptions
            {
                Name = name,
                BoardId = boardId,
                GroupId = groupId,
                ColumnValues = columnValues,
                CreateLabelsIfMissing = false
            };

            if (!TryCreateItem(options, out var item, out var error))
            {
                errorMessages.Add(error);
                action.AddLogEntry(error);
                if (!GetAttributeValue(action, "ContinueOnError").AsBoolean())
                {
                    return false;
                }

                return true;
            }

            action.AddLogEntry($"Monday.com Item({item.Id}) has been created.");
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

        private bool TryCreateItem(ItemCreationOptions options, out Utilities.Api.Schema.Item item, out string error)
        {
            var api = new MondayApi();
            item = api.CreateItem(options);

            if (item == null)
            {
                error = "Could not create Monday.com Item";
                return false;
            }
            error = null;
            return true;
        }
    }
}
