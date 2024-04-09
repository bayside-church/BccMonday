using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.Utilities.Api.Config
{
    internal class MondayOptions
    {
    }

    public class StatusColumnOptions
    {
        [JsonProperty("done_colors")]
        public List<int> DoneColors { get; set; }

        [JsonProperty("labels")]
        public Dictionary<int, string> Labels { get; set; }

        [JsonProperty("labels_positions_v2")]
        public Dictionary<int, string> LabelsPositions { get; set; }

        [JsonProperty("labels_colors")]
        public Dictionary<int, LabelColor> LabelsColor { get; set; }
    }

    public class  LabelColor
    {
        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("border")]
        public string Border { get; set; }

        [JsonProperty("var_name")]
        public string VarName { get; set; }
    }

    public class ColumnCreationOptions
    {
        /// <summary>
        /// the unique identifier of the column after which the new column will be created.
        /// </summary>
        public string AfterColumnId { get; set; }

        /// <summary>
        /// The board's unique identifier where the new column should be created,
        /// </summary>
        public long BoardId { get; set; }

        /// <summary>
        /// The type of column to create.
        /// </summary>
        public string ColumnType { get; set; }

        /// <summary>
        /// The new column's defaults.
        /// </summary>
        public string Defaults { get; set; }

        /// <summary>
        /// The new column's description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The column's user-specified unique identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The new column's title.
        /// </summary>
        public string Title { get; set; }
    }

    public class ColumnChangeOptions
    {
        /// <summary>
        /// The unique identifier of the board that contains the column to change.
        /// </summary>
        public long BoardId { get; set; }

        /// <summary>
        /// The column's unique identifier
        /// </summary>
        public string ColumnId { get; set; }

        /// <summary>
        /// Creates status/dropdown labels if they are missing. Requires permission to change the board structure.
        /// </summary>
        public bool CreateLabelsIfMissing { get; set; }

        /// <summary>
        /// The unique identifier of the item to change.
        /// </summary>
        public long ItemId { get; set; }

        /// <summary>
        /// The new value of the column in JSON format.
        /// </summary>
        public string Value { get; set; }
    }

    public class ColumnChangeSimpleValueOptions
    {
        /// <summary>
        /// The unique identifier of the board that contains the column to change.
        /// </summary>
        public long BoardId { get; set; }

        /// <summary>
        /// The column's unique identifier
        /// </summary>
        public string ColumnId { get; set; }

        /// <summary>
        /// Creates status/dropdown labels if they are missing. Requires permission to change the board structure.
        /// </summary>
        public bool CreateLabelsIfMissing { get; set; }

        /// <summary>
        /// The unique identifier fo the item to change.
        /// </summary>
        public long ItemId { get; set; }

        /// <summary>
        /// The new simple value of the column.
        /// </summary>
        public string Value { get; set; }
    }

    public class ColumnChangeMultipleValuesOptions
    {
        /// <summary>
        /// The unique identifier of the board that contains the columns to change.
        /// </summary>
        public long BoardId { get; set; }

        /// <summary>
        /// The updated column values
        /// </summary>
        public string ColumnValues { get; set; }

        /// <summary>
        /// Creates status/dropdown labels if they are missing. Requires permission to change the board structure.
        /// </summary>
        public bool CreateLabelsIfMissing { get; set; }

        /// <summary>
        /// The unique identifier of the item to change.
        /// </summary>
        public long ItemId { get; set; }
    }

    public class ColumnChangeTitleOptions
    {
        /// <summary>
        /// The unique identifier of the board that contains the column to change.
        /// </summary>
        public long BoardId { get; set; }

        /// <summary>
        /// The column's unique identifier.
        /// </summary>
        public string ColumnId { get; set; }

        /// <summary>
        /// The column's new title.
        /// </summary>
        public string Title { get; set; }
    }

    public class ColumnChangeMetadataOptions
    {
        /// <summary>
        /// The unique identifier of the board that contains the column to change.
        /// </summary>
        public long BoardId { get; set; }

        /// <summary>
        /// The column's unique identifier.
        /// </summary>
        public string ColumnId { get; set; }

        /// <summary>
        /// The property you want to change: title or description.
        /// </summary>
        public string ColumnProperty { get; set; }

        /// <summary>
        /// The new value of that property.
        /// </summary>
        public string Value { get; set; }
    }

    public class ColumnDeleteOptions
    {
        /// <summary>
        /// The unique identifier of the board that contains the column to delete.
        /// </summary>
        public long BoardId { get; set; }

        /// <summary>
        /// The column's unique identifier.
        /// </summary>
        public string ColumnId { get; set; }
    }

    #region item options
    public class ItemCreationOptions
    {
        /// <summary>
        /// The board's unique identifier.
        /// </summary>
        public long BoardId { get; set; }

        /// <summary>
        /// The column values of the new item.
        /// </summary>
        public Dictionary<string, string> ColumnValues { get; set; }

        /// <summary>
        /// Creates status/dropdown labels if they are missing (requires permission to change the board structure).
        /// </summary>
        public bool CreateLabelsIfMissing { get; set; }

        /// <summary>
        /// The group's unique identifier
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// The new item's name
        /// </summary>
        public string Name { get; set; }
    }

    public class ItemDuplicateOptions
    {
        /// <summary>
        /// The board's unique identifier.
        /// </summary>
        public long BoardId { get; set; }

        /// <summary>
        /// The item's unique identifier. Required.
        /// </summary>
        public long ItemId { get; set; }

        /// <summary>
        /// Duplicates the item with existing updates.
        /// </summary>
        public bool WithUpdates { get; set; }
    }

    public class ItemMoveToGroupOptions
    {
        /// <summary>
        /// The group's unique identifier.
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// The item's unique identifier.
        /// </summary>
        public long ItemId { get; set; }
    }

    public class ItemMoveToBoardOptions
    {
        /// <summary>
        /// The unique identifier of the board to move the item to (target board)
        /// </summary>
        public long BoardId { get; set; }

        /// <summary>
        /// The unique identifier of the group to move the item to (target group)
        /// </summary>
        public long GroupId { get; set; }

        /// <summary>
        /// The unique identifier of the item to move.
        /// </summary>
        public long ItemId { get; set; }

        /// <summary>
        /// The object that defines the column mapping between the original and target board. If you omit this argument, the columsn will be mapped based on the best match,
        /// </summary>
        public List<string> ColumnsMapping { get; set; }

        /// <summary>
        /// The object that defines the subitems' column mapping between the original and target board. If you omit this argument, the columsn will be mapped based on the best match.
        /// </summary>
        public List<string> SubitemsColumnMapping { get; set; }
    }

    public class ItemArchiveOptions
    {
        /// <summary>
        /// The item's unique identifier.
        /// </summary>
        public long ItemId { get; set; }
    }

    public class ItemDeleteOptions
    {
        /// <summary>
        /// The item's unique identifier.
        /// </summary>
        public long ItemId { get; set; }
    }

    public class ItemClearUpdatesOptions
    {
        /// <summary>
        /// The item's unique identifier.
        /// </summary>
        public long ItemId { get; set; }
    }

    public class SubItemCreationOptions
    {
        /// <summary>
        /// The column values of the new subitem.
        /// </summary>
        public string ColumnValues { get; set; }

        /// <summary>
        /// Creates status/dropdown labels if they're missing. Requires permission to change the board structure.
        /// </summary>
        public bool CreateLabelsIfMissing { get; set; }

        /// <summary>
        /// The new subitem's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the parent item's unique identifier.
        /// </summary>
        public long ParentItemId { get; set; }
    }

    #endregion

    #region updates

    public class UpdateCreationOptions
    {
        /// <summary>
        /// The update's text.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// The item's unique identifier.
        /// </summary>
        public long ItemId { get; set; }

        /// <summary>
        /// The parent update's unique identifier. This can be used to create a reply to an update.
        /// </summary>
        public long ParentId { get; set; }
    }

    public class UpdateClearAllOptions
    {
        /// <summary>
        /// The item's unique identifier.
        /// </summary>
        public long ItemId { get; set; }
    }

    public class UpdateDeleteOptions
    {
        /// <summary>
        /// The update's unique identifier.
        /// </summary>
        public long Id { get; set; }
    }

    #endregion

    #region Board

    public class BoardCreationOptions
    {
        /// <summary>
        /// The type of board to create: public, private, or share.
        /// </summary>
        public string BoardKind { get; set; }

        /// <summary>
        /// The new board's name.
        /// </summary>
        public string BoardName { get; set; }

        public List<long> BoardOwnerIds { get; set; }

        public List<long> BoardSubscriberIds { get; set; }

        public List<long> BoardSubscriberTeamIds { get; set; }

        public string Description { get; set; }

        public long FolderId { get; set; }

        public long TemplateId { get; set; }

        public long WorkspaceId { get; set; }
    }

    public class BoardDuplicateOptions
    {
        /// <summary>
        /// The board's unique identifier.
        /// </summary>
        public long BoardId { get; set; }

        /// <summary>
        /// The duplicated board's nam. If omitted, it will be automatically generated.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The duplication type: duplicate_board_with_structure, duplicate_board_with_pulses, or duplicate_board_with_pulses_and_updates.
        /// </summary>
        public string DuplicateType { get; set; }

        /// <summary>
        /// The destiantion folder within the destination workspace. The folder_id is required if you duplicating to another workspace, otherwise, it is optional. If omitted, it will default to the original board's folder.
        /// </summary>
        public long FolderId { get; set; }

        /// <summary>
        /// Ability to duplicate the subscribers to the new board. Defaults to false.
        /// </summary>
        public bool KeepSubscribers { get; set; }

        /// <summary>
        /// The destination workspace. If omitted, it will default to the original board's workspace.
        /// </summary>
        public long WorkspaceId { get; set; }
    }

    public class BoardUpdateOptions
    {
        /// <summary>
        /// The board's attribute to update: name, description, or communication.
        /// </summary>
        public string BoardAttribute { get; set; }

        /// <summary>
        /// The board's unique identifier.
        /// </summary>
        public long BoardId { get; set; }

        /// <summary>
        /// The new attribute value
        /// </summary>
        public string NewValue { get; set; }
    }

    public class BoardArchiveOptions
    {
        /// <summary>
        /// The board's unique identifier.
        /// </summary>
        public long BoardId { get; set; }
    }

    public class BoardDeleteOptions
    {
        /// <summary>
        /// The board's unique identifier.
        /// </summary>
        public long BoardId { get; set; }
    }

    #endregion

    #region Groups

    public class GroupCreationOptions
    {
        /// <summary>
        /// The board's unique identifier.
        /// </summary>
        public long BoardId { get; set; }

        /// <summary>
        /// The group's HEX code color.
        /// </summary>
        public string GroupColor { get; set; }

        /// <summary>
        /// The new group's name.
        /// </summary>
        public string Name { get; set; }
    }

    public class GroupUpdateOptions
    {
        /// <summary>
        /// The board's unique identifier
        /// </summary>
        public long BoardId { get; set; }

        /// <summary>
        /// The group attribute that you want to update. The accepted attribtues are title, color, relative_position_after, and relative_position_before.
        /// </summary>
        public string GroupAttribute { get; set; }

        /// <summary>
        /// The grouop's unique identifier
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// The new attribute value
        /// </summary>
        public string NewValue { get; set; }
    }

    public class GroupDuplicateOptions
    {
        /// <summary>
        /// Boolean to add the new group to the top of the board.
        /// </summary>
        public bool AddToTop { get; set; }

        /// <summary>
        /// The board's unique identifier.
        /// </summary>
        public long BoardId { get; set; }

        /// <summary>
        /// The group's unique identifier.
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// The group's title.
        /// </summary>
        public string Title { get; set; }
    }

    public class GroupArchiveOptions
    {
        /// <summary>
        /// The board's unique identifier.
        /// </summary>
        public long BoardId { get; set; }

        /// <summary>
        /// The group's unique identifier.
        /// </summary>
        public string GroupId { get; set; }
    }

    public class GroupDeleteOptions
    {
        /// <summary>
        /// The board's unique identifier.
        /// </summary>
        public long BoardId { get; set; }

        /// <summary>
        /// The group's unique identifier.
        /// </summary>
        public string GroupId { get; set; }
    }
    #endregion

}
