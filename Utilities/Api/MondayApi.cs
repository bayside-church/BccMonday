using com.baysideonline.BccMonday.Utilities.Api.Config;
using com.baysideonline.BccMonday.Utilities.Api.Interfaces;
using com.baysideonline.BccMonday.Utilities.Api.Responses;
using com.baysideonline.BccMonday.Utilities.Api.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public enum MondayApiType
    {
        Standard,
        File
    }

    public class MondayApiUrls
    {
        public static readonly Dictionary<MondayApiType, string> Urls = new Dictionary<MondayApiType, string>
        {
            { MondayApiType.Standard, "https://api.monday.com/v2" },
            { MondayApiType.File, "https://api.monday.com/v2/file" }
        };
    }

    public class MondayApi : IMondayApi
    {
        private readonly string _apiUrl;
        private string _apiKey;
        protected IRestClient _client;
        protected IRestRequest _request;
        protected bool _isInitialized = false;

        public MondayApi(MondayApiType apiType = MondayApiType.Standard)
        {
            if (!MondayApiUrls.Urls.TryGetValue(apiType, out _apiUrl))
            {
                throw new ArgumentException("Invalid API Type specified", nameof(apiType));
            }
        }

        public MondayInitializeResponse Initialize()
        {
            // return if already initialized
            if (_isInitialized)
                return new MondayInitializeResponse()
                {
                    Message = "Already initialized",
                    Status = MondayStatuses.OK
                };

            // get the key
            using (var context = new RockContext())
            {
                IMondayApiKey keyHelper = new MondayApiKey(context);
                _apiKey = keyHelper.Get();

                if (_apiKey == null)
                {
                    ExceptionLogService.LogException(new Exception("The Monday.com API Key is missing.", new Exception("BccMonday")));
                    return new MondayInitializeResponse()
                    {
                        Status = MondayStatuses.ERROR,
                        Message = "Unable to obtain monday.com API key"
                    };
                }
            }

            // initialize rest client & rest request
            _client = new RestClient(_apiUrl);
            _request = new RestRequest("/")
                .AddHeader("Authorization", _apiKey);

            _isInitialized = true;

            return new MondayInitializeResponse()
            {
                Status = MondayStatuses.OK,
                Message = "Initialized"
            };
        }

        #region mutations

        public Update AddUpdateToItem(long itemId, string body, long? parentUpdateId = null)
        {
            if (!Initialize().IsOk())
                return null;

            var query = parentUpdateId != null
                ? @"mutation ($itemId: ID, $body: String!, $parentUpdateId: ID){
                        create_update (item_id: $itemId, body: $body, parent_id: $parentUpdateId) {
                            id
                            body
                            text_body
                            created_at
                            creator_id
                            creator {
                                id
                                name
                            }
                        }
                    }"
                : @"mutation ($itemId: ID, $body: String!) {
                        create_update (item_id: $itemId, body: $body) {
                            id
                            body
                            text_body
                            created_at
                            creator_id
                            creator {
                                id
                                name
                            }
                        }
                    }";

            var variables = new Dictionary<string, object>()
            {
                { "itemId", itemId },
                { "body", body },
                { "parentUpdateId", parentUpdateId }
            };

            var queryData = Query<CreateUpdateResponse>(query, variables);
            var update = queryData.Update;
            return update;
        }

        public StatusColumnValue ChangeColumnValue(long boardId, long itemId, string columnId, string newValue)
        {
            if (!Initialize().IsOk())
                return null;

            string query = @"
                mutation ($boardId: ID!, $columnId: String!, $itemId: ID, $newValue: String){
                    change_simple_column_value(
                        board_id: $boardId
                        column_id: $columnId
                        item_id: $itemId
                        value: $newValue)
                        {
                            id
                            column_values(ids: [$columnId]) {
                                id
                                text
                                type
                                value
                                ... on StatusValue {
                                    label_style {
                                        color
                                    }
                                }
                            }
                        }
                    }";

            var variables = new Dictionary<string, object>()
            {
                { "boardId", boardId },
                { "columnId", columnId },
                { "itemId", itemId },
                { "newValue", newValue }
            };

            var queryData = Query<ChangeSimpleColumnValueResponse>(query, variables);
            var item = queryData.Item;
            var columnValue = item.ColumnValues[0];

            return columnValue as StatusColumnValue;
        }

        public Asset AddFileToUpdate(long updateId, BinaryFile binaryFile)
        {
            if (!Initialize().IsOk())
            {
                return null;
            }

            var filePath = new Uri(GlobalAttributesCache.Get().GetValue("PublicApplicationRoot"))
                + $"GetFile.ashx?id={binaryFile.Id}";
            var fileName = binaryFile.FileName;
            var fileSize = binaryFile.FileSize;
            const long MAX_BYTES = 100000000;

            if (fileSize > MAX_BYTES)
            {
                return null;
            }

            using (WebClient webClient = new WebClient())
            {
                var bytes = webClient.DownloadData(filePath);
                string query = @"
                        mutation ($file: File!, $updateId: ID!) {
                            add_file_to_update(file: $file, update_id: $updateId) {
                                id
                                file_size
                                name
                                public_url
                                url_thumbnail
                            }
                        }";

                var variables = new Dictionary<string, object>
                {
                    { "updateId", updateId }
                };

                var data = FileQuery<AddFileToUpdateResponse>(query, bytes, fileName, variables);
                if (data == null) return null;
                var asset = data.Asset;

                return asset;
            }
        }

        public Asset AddFileToColumn(long itemId, string columnId, BinaryFile file)
        {
            throw new NotImplementedException();
        }

        public MondayUser AddUserToBoard(long boardId, long userId, string kind)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add user to a workspace.
        /// </summary>
        /// <param name="workspaceId"></param>
        /// <param name="userId"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public MondayUser AddUserToWorkspace(long workspaceId, long userId, string kind)
        {
            throw new NotImplementedException();
        }

        #region Archive Entities
        /// <summary>
        /// Archives a group in a specific board.
        /// </summary>
        /// <param name="boardId">The board's unique identifier</param>
        /// <param name="groupId">The group's unique identifier</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IMondayGroup ArchiveGroup(long boardId, string groupId)
        {
            string query = @"
                mutation ($boardId: ID!, $groupId: String!) {
                    archive_group(board_id: $boardId, group_id: $groupId) {
                        id
                    }
                }";

            throw new NotImplementedException();
        }

        /// <summary>
        /// Archives a board.
        /// </summary>
        /// <param name="boardId">The board's unique identifier</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Board ArchiveBoard(long boardId)
        {
            string query = @"
                mutation ($boardId: ID!) {
                    archive_board(board_id: $boardId) {
                        id
                    }
                }";

            throw new NotImplementedException();
        }
        #endregion
        /// <summary>
        /// Change a column's properties
        /// </summary>
        /// <param name="columnId">The column's unique identifier</param>
        /// <param name="boardId">The board's unique identifier</param>
        /// <param name="columnProperty">The property name of the column to be changed (title / description)</param>
        /// <param name="value">The new description of the column</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Column ChangeColumnMetadata(string columnId, long boardId, string columnProperty, string value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Change an item's column value
        /// </summary>
        /// <param name="itemId">The item's unique identifier</param>
        /// <param name="columnId">The column's unique identifier</param>
        /// <param name="boardId">The board's unique identifier</param>
        /// <param name="json">The new value of the column</param>
        /// <param name="createLabelsIfMissing">Create Status/Dropdown labels if they're missing. (Requires permission to change board structure)</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        //public Item ChangeColumnValue(long itemId, string columnId, long boardId, string json, bool createLabelsIfMissing)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Changes the column values of a specific item.
        /// </summary>
        /// <param name="itemId">The item's unique identifier</param>
        /// <param name="boardId">The board's unique identifier</param>
        /// <param name="columnValues">The column values updates</param>
        /// <param name="createLabelsIfMissing">Create Status/Dropdown labels if they're missing. (Requires permission to change board structure)</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Item ChangeMultipleColumnValues(long itemId, long boardId, string columnValues, bool createLabelsIfMissing)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clear an item's updates
        /// </summary>
        /// <param name="itemId">The item's unique identifier</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Item ClearItemUpdates(long itemId)
        {
            string query = @"
                mutation ($itemId: ID!) {
                    clear_item_updates(item_id: $itemId) {
                        id
                    }
                }";

            throw new NotImplementedException();
        }
        #region Create Entities
        public Board CreateBoard(string boardName, string description, string boardKind, long folderId, long workspaceId, long templateId,
            List<string> boardOwnerIds, List<string> boardOwnerTeamIds, List<string> boardSubscriberIds, List<string> boardSubscriberTeamIds, bool empty)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create a new column in board.
        /// </summary>
        /// <param name="boardId">The board's unique identifier</param>
        /// <param name="title">The new column's title</param>
        /// <param name="description">The new column's description</param>
        /// <param name="columnType">The type of column to create</param>
        /// <param name="defaults">The new column's defaults</param>
        /// <param name="id">The column's user-specified unique identifier</param>
        /// <param name="afterColumnId">The column's unique identifier after which the new column will be inserted</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Column CreateColumn(long boardId, string title, string description, string columnType, string defaults, string id, string afterColumnId)
        {
            string query = @"
                mutation ($boardId: ID!, title: String!, $description: String,
                        $columnType: ColumnType!, $defaults; JSON,
                        $id: String, $afterColumnId: ID) {
                    create_column(board_id: $boardId, title: $title,
                        description: $description, column_type: $columnType,
                        defaults: $defaults, id: $id, after_column_id: $afterColumnId) {
                        id
                    }
                }";
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new group in a specific board.
        /// </summary>
        /// <param name="boardId">The board's unique identifier</param>
        /// <param name="groupName">The name of the new group</param>
        /// <param name="relativeTo">The group to set the position next to</param>
        /// <param name="positionRelativeMethod">The position relative method to another group (before_at / after_at)</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IMondayGroup CreateGroup(long boardId, string groupName, string relativeTo, string positionRelativeMethod)
        {
            string query = @"
                mutation ($boardId: ID!, $groupName: String!, $relativeTo: String, $positionRelativeMethod: PositionRelative) {
                    create_group(board_id: $boardId, group_name: $groupName, $relative_to: $relativeTo, position_relative_method: $positionRelativeMethod) {
                        id
                    }
                }";
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new item.
        /// </summary>
        /// <param name="itemName">The new item's name.</param>
        /// <param name="boardId">The board's unique identifier</param>
        /// <param name="groupId">The group's unique identifier</param>
        /// <param name="columnValues">The column values of the new item</param>
        /// <param name="createLabelsIfMissing">Create Status/Dropdown labels if they're missing. (Requires permission to change board structure)</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Item CreateItem(string itemName, long boardId, string groupId, string columnValues, bool createLabelsIfMissing)
        {
            string query = @"
                mutation ($itemName: String!, $board_id: ID!, $groupId: String, $columnValues: JSON, $createLabelsIfMissing: Boolean) {
                    create_item(item_name: $itemName, board_id: $boardId, group_id: $groupId, $column_values: $columnValues, create_labels_if_missing: $createLabelsIfMissing) {
                        id
                    }
                }";
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create subitem
        /// </summary>
        /// <param name="parentItemId">The parent item's unique identifier</param>
        /// <param name="itemName">The new item's name</param>
        /// <param name="columnValues">The column values of the new item.</param>
        /// <param name="createLabelsIfMissing">Create Status/Dropdown labels if they're missing. (Requires permission to change board structure)</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Item CreateSubitem(long parentItemId, string itemName, string columnValues, bool createLabelsIfMissing)
        {
            string query = @"
                mutation ($parentItemId: ID!, $itemName: String!, $columnValues: JSON, $createLabelsIfMissing: Boolean) {
                    create_subitem(parent_item_id: $parentItemId, item_name: $itemName, $column_values: $columnValues, create_labels_if_missing: $createLabelsIfMissing) {
                        id
                    }
                }";
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create a new workspace
        /// </summary>
        /// <param name="name"></param>
        /// <param name="kind"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Workspace CreateWorkspace(string name, string kind, string description)
        {
            string query = @"
                mutation ($name: String!, $kind: WorkspaceKind!, $description: String) {
                    create_workspace(name: $name, kind: $kind, description: $description) {
                        id
                    }
                }";
            throw new NotImplementedException();
        }
        #endregion
        #region Delete Entities
        /// <summary>
        /// Delete a board.
        /// </summary>
        /// <param name="boardId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Board DeleteBoard(long boardId)
        {
            string query = @"
                mutation ($boardId: ID!) {
                    delete_board(board_id: $boardId) {
                        id
                    }
                }";
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete a column.
        /// </summary>
        /// <param name="boardId"></param>
        /// <param name="columnId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Column DeleteColumn(long boardId, string columnId)
        {
            string query = @"
                mutation ($boardId: ID!, $columnId: String!) {
                    delete_column(board_id: $boardId, column_id: $columnId) {
                        id
                    }
                }";
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete a group in a specific board.
        /// </summary>
        /// <param name="boardId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IMondayGroup DeleteGroup(long boardId, string groupId)
        {
            string query = @"
                mutation ($boardId: ID!, $groupId: String!) {
                    delete_group(board_id: $boardId, group_id: $groupId) {
                        id
                    }
                }";
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete an item.
        /// </summary>
        /// <param name="itemID">The item's unique identifier</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Item DeleteItem(long itemID)
        {
            string query = @"
                mutation ($itemId: ID) {
                    delete_item(item_id: $itemId) {
                        id
                    }
                }";
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete an update.
        /// </summary>
        /// <param name="updateId">The update's unique identifier</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Update DeleteUpdate(long updateId)
        {
            string query = @"
                mutation ($updateId: ID!) {
                    delete_update(id: $updateId) {
                        id
                    }
                }";
            throw new NotImplementedException();
        }
        #endregion
        #region DuplicatEntities
        /// <summary>
        /// Delete workspace.
        /// </summary>
        /// <param name="workspaceId">The workspace's unique identifier</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Workspace DeleteWorkspace(long workspaceId)
        {
            string query = @"
                mutation ($workspaceId: ID!) {
                    delete_workspace(workspace_id: $workspaceId) {
                        id
                    }
                }";
            throw new NotImplementedException();
        }

        /// <summary>
        /// Duplicate a board.
        /// </summary>
        /// <param name="boardId">The board's unique identifier</param>
        /// <param name="duplicateType">The duplication type.</param>
        /// <param name="name">(Optional) The new board's name. If omitted then automatically generated.</param>
        /// <param name="workspaceId">Optional destination workspace. Defaults to the original board workspace.</param>
        /// <param name="folderId">Optional destination folder in destination workspace. Defaults to the original board folder.</param>
        /// <param name="keepSubscribers">Duplicate the subscribers to the new board. Defaults to false.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Board DuplicateBoard(long boardId, string duplicateType, string name, string workspaceId, string folderId, bool keepSubscribers)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete a group.
        /// </summary>
        /// <param name="boardId">The board's unique identifier</param>
        /// <param name="groupId">The group's unique identifier</param>
        /// <param name="addToTop">Should the new group be added to the top.</param>
        /// <param name="groupTitle">The group's title.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IMondayGroup DuplicateGroup(long boardId, string groupId, bool addToTop, string groupTitle)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Duplicate an item.
        /// </summary>
        /// <param name="boardId">The board's unique identifier</param>
        /// <param name="withUpdates">Duplicate with the item's updates</param>
        /// <param name="itemId">The item's unique identifier. *Required</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Item DuplicateItem(long boardId, bool withUpdates, long itemId)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Move Entities
        /// <summary>
        /// Move an item to a different board.
        /// </summary>
        /// <param name="boardId">The unique identifier of a target board.</param>
        /// <param name="groupId">The unique identifier of a target group.</param>
        /// <param name="itemId">The unique identifier of an item to move.</param>
        /// <param name="columnsMapping">Mapping of colums between the original board and target board</param>
        /// <param name="subitemsColumnsMapping">Mapping of subitme columns between the original board and target board</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Item MoveItemToBoard(long boardId, string groupId, long itemId, string columnsMapping, string subitemsColumnsMapping)
        {
            throw new NotImplementedException();
        }

        
        public Item MoveItemToGroup() {  throw new NotImplementedException(); }
        #endregion
        #region Update Entities
        /// <summary>
        /// Update Board attribute
        /// </summary>
        /// <param name="boardId">The board's unique identifier</param>
        /// <param name="boardAttribute">The board's attribute to update (name / description / communication)</param>
        /// <param name="newValue">The new attribute value</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string UpdateBoard(long boardId, string boardAttribute, string newValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update an existing group.
        /// </summary>
        /// <param name="boardId">The board's unique identifier</param>
        /// <param name="groupId">The Group's unique identifier</param>
        /// <param name="groupAttribute">The group's attribute to update (title / color / position / relative_position_afer / relative_position_before)</param>
        /// <param name="newValue">The new attribute value</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string UpdateGroup(long boardId, string groupId, string groupAttribute, string newValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update an exisintg workspace.
        /// </summary>
        /// <param name="id">The workspace ID.</param>
        /// <param name="attributes">The attributes of the workspace to update</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Workspace UpdateWorkspace(long id, string attributes)
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion

        #region queries

        public List<Asset> GetFilesByAssetIds(List<long> ids)
        {
            if (!Initialize().IsOk())
                return null;

            var query = @"
                query ($assetIds: [ID!]!) {
                    assets (ids: [$assetIds]) {
                        id
                        public_url
                        name
                        file_size
                        url_thumbnail
                    }
                }";

            var variables = new Dictionary<string, object>()
            {
                { "assetIds", string.Join(",", ids)}
            };

            var queryData = Query<GetAssetsResponse>(query, variables);
            if (queryData == null) return null;
            var assets = queryData.Assets;
            if (assets.Count <= 0 ) return null;
            return assets;
        }

        public Board GetBoard(long id)
        {
            if (!Initialize().IsOk())
                return null;

            var query = @"
                query ($boardId: ID!) {
                    boards (ids: [$boardId] limit:1) {
  	                    name
  	                    id
                        columns {
                            id
                            title
                            type
                            settings_str
                        }
	                }
                }
            ";
            var variables = new Dictionary<string, object>()
            {
                { "boardId", id }
            };

            var queryData = Query<GetBoardsResponse>(query, variables);
            if (queryData == null) return null;
            var boards = queryData.Boards;

            if (boards.Count <= 0) return null;
            return boards[0];
        }

        public string GetWorkspace(long boardId)
        {
            if (!Initialize().IsOk())
            {
                return null;
            }
            var query = @"
                query ($boardId: ID!){
                    boards(ids: [$boardId] ) {
                        id
                        name
                        workspace {
                            id
                            name
                        }
                    }
                }";
            var variables = new Dictionary<string, object>()
            {
                { "boardId", boardId }
            };

            var queryData = Query<GetBoardsResponse>(query, variables);//GetBoardWorkspaceResponse>(query, variables);
            var board = queryData.Boards[0];
            var workspace = board.Workspace;
            return workspace.Name ?? null;
        }

        public List<Board> GetBoards()
        {
            if (!Initialize().IsOk())
                return null;

            string query = @"
                query {
                    boards(limit: 500) {
                        id
                        name
                        type
                    }
                }";

            var queryData = Query<GetBoardsResponse>(query);
            if (queryData == null) return null;
            var boards = queryData.Boards;
            boards = boards.Where(b => b.BoardType == "board").ToList();
            return boards;
        }

        public Item GetItem(long id)
        {
            if (!Initialize().IsOk())
                return null;

            var query = @"
                query ($itemId: ID!) {
                  items(ids: [$itemId], limit: 1) {
                    id
                    name
                    created_at
                    board {
                      id
                      name
                    }
                    column_values {
                      id
                      text
                      type
                      value
                      ... on ButtonValue {
                        id
                        color
                        buttonLabel: label
                      }
                      ... on MirrorValue {
                        display_value
                      }
                      ... on BoardRelationValue {
                        value
                        display_value
                        linked_items {
                          id
                          relative_link
                        }
                        linked_item_ids
                      }
                      ... on DateValue {
                        id
                        date
                        time
                      }
                      ... on StatusValue {
                        id
                        value
                        index
                        statusLabel: label
                        is_done
                        label_style {
                          color
                          border
                        }
                      }
                      ... on EmailValue {
                        id
                        email
                      }
                      ... on FileValue {
                        id
                        files {
                          ... on FileDocValue {
                            file_id
                            url
                          }
                          ... on FileAssetValue {
                            asset_id
                            asset {
                              url
                              name
                              url_thumbnail
                              public_url
                            }
                          }
                        }
                      }
                      column {
                        id
                        title
                        settings_str
                        description
                        type
                      }
                    }
                    updates {
                      id
                      body
                      text_body
                      created_at
                      creator_id
                      assets {
                        id
                        name
                        file_size
                        public_url
                        url_thumbnail
                      }
                      creator {
                        id
                        name
                      }
                      replies {
                        id
                        body
                        text_body
                        created_at
                        creator_id
                        creator {
                          id
                          name
                        }
                      }
                    }
                  }
                }
            ";

            var variables = new Dictionary<string, object>()
            {
                { "itemId", id }
            };

            var queryData = Query<GetItemsResponse>(query, variables);
            if (queryData == null) return null;
            var items = queryData.Items;

            if (items.Count == 0) return null;

            var item = items[0];
            return item;
        }

        public List<Item> GetItemsByBoardAndColumnValues(long boardId, ItemsPageByColumnValuesQuery columnValues)
        {
            if (!Initialize().IsOk()) return null;

            List<Item> allItems = new List<Item>();

            var initialQuery = @"
                query($boardId: ID!, $columnValues: [ItemsPageByColumnValuesQuery!]) {
                    items_page_by_column_values(limit: 500, board_id: $boardId, column_values: $columnValues) {
                        cursor
                        items {
                            id
                            name
                            created_at
                            column_values {
                                id
                                text
                                type
                                value
                                column {
                                    id
                                    title
                                    settings_str
                                }
                            }
                        }
                    }
                }";

            var variables = new Dictionary<string, object>()
            {
                { "boardId", boardId },
                { "columnValues", columnValues }
            };

            var initialData = Query<GetItemsPageResponse>(initialQuery, variables);
            var itemsPage = initialData.ItemsPage;
            var items = itemsPage.Items.ConvertAll(i => (Item)i);
            var cursor = itemsPage.Cursor;
            allItems.AddRange(items);

            throw new NotImplementedException();
        }

        public List<Item> GetItemsByBoard(long boardId, string emailMatchColumnId, string statusColumnId)
        {
            if (!Initialize().IsOk())
                return null;

            List<Item> allItems = new List<Item>();

            // Initial query
            var initialQuery = @"
    query( $boardId: ID!, $emailColumnId: String!, $statusColumnId: String!) {
        boards(ids: [$boardId], limit: 1) {
            id
            items_page(limit: 500) {
                cursor
                items {
                    id
                    name
                    created_at
                    column_values(ids: [ $emailColumnId, $statusColumnId ]) {
                        id
                        text
                        type
                        value
                        column {
                            id
                            title
                            settings_str
                        }
                        ... on MirrorValue {
                            display_value
                        }
                        ... on StatusValue {
                        id
                        value
                        index
                        statusLabel: label
                        is_done
                        label_style {
                          color
                          border
                        }
                      }
                    }
                }
            }
        }
    }";

            var variables = new Dictionary<string, object>()
            {
                { "boardId", boardId },
                { "emailColumnId", emailMatchColumnId },
                { "statusColumnId", statusColumnId }
            };
            var initialQueryData = Query <GetBoardsResponse> (initialQuery, variables);
            var board = initialQueryData.Boards[0];
            var itemsPage = board.ItemsPage;
            string cursor = itemsPage.Cursor;

            // Process initial items
            var initialItems = itemsPage.Items.ConvertAll(i => (Item)i);
            if (initialItems != null)
            {
                allItems.AddRange(initialItems);
            }

            while (!string.IsNullOrEmpty(cursor))
            {
                var nextItemsQuery =
                    @"query ($cursorVal: String, $emailColumnId: String!, $statusColumnId: String!) {
                        next_items_page(cursor: $cursorVal, limit: 1) {
                            cursor
                            items {
                                id
                                name
                                created_at
                                column_values(ids: [$emailColumnId, $statusColumnId]) {
                                    id
                                    text
                                    type
                                    value
                                    column {
                                        id
                                        title
                                        settings_str
                                    }
                                    ... on StatusValue {
                                        id
                                        value
                                        index
                                        statusLabel: label
                                        is_done
                                        label_style {
                                            color
                                            border
                                        }
                                    }
                                }
                            }
                        }
                    }";

                variables = new Dictionary<string, object>()
                {
                    { "cursorVal", cursor },
                    { "emailColumnId", emailMatchColumnId },
                    { "statusColumnId", statusColumnId }
                };

                var nextItemsPageData = Query<GetNextItemsPageResponse>(nextItemsQuery, variables);
                if (nextItemsPageData == null) return allItems;
                var nextItemsPage = nextItemsPageData.NextItemsPage;
                cursor = nextItemsPage.Cursor;

                var nextItems = nextItemsPage.Items.ConvertAll(i => (Item)i);
                allItems.AddRange(nextItems);
            }

            return allItems;
        }

        #endregion
        #region private methods
        private T Query<T>(string query, object variables = null)
        {
            if (_isInitialized)
            {
                _request.AddJsonBody(new { query, variables });
                _request.AddHeader("API-Version", "2024-01");

                var res = _client.Post(_request);

                if (res.StatusCode == HttpStatusCode.OK)
                {
                    var content = res.Content;
                    var queryData = JsonConvert.DeserializeObject<GraphQLResponse<T>>(content);

                    if (queryData.Data != null)
                    {
                        return queryData.Data;
                    }
                    else if (queryData.Errors != null && queryData.Errors.Count > 0)
                    {
                        string errorMessage = queryData.Errors[0].Message;
                        ExceptionLogService.LogException(new Exception($"{errorMessage} | query: {query}", new Exception("BccMonday")));
                    }
                }
                else if (res.StatusCode == HttpStatusCode.InternalServerError)
                {
                    string errorMessage = $"Monday.com is having technical issues. Your API Request did not go through. Query: {query}";
                    ExceptionLogService.LogException(new Exception(errorMessage, new Exception("BccMonday")));
                }
            }

            return default;
        }

        private T FileQuery<T>(string query, byte[] bytes, string fileName, object variables = null)
        {
            _request.AddHeader("Content-Type", "multipart/form-data");
            _request.AddJsonBody(new { query, variables });
            _request.AddFile("variables[file]", bytes, fileName);
            var response = _client.Execute(_request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var queryData = JsonConvert.DeserializeObject<GraphQLResponse<T>>(response.Content);

                if (queryData.Data != null)
                {
                    return queryData.Data;
                }
                else if (queryData.Errors != null && queryData.Errors.Count > 0)
                {
                    string errorMessage = queryData.Errors[0].Message;
                    ExceptionLogService.LogException(new Exception($"{errorMessage} | query: {query}", new Exception("BccMonday")));
                }
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                string errorMessage = $"Monday.com is having technical issues. Your API Request did not go through. Query: {query}";
                ExceptionLogService.LogException(new Exception(errorMessage, new Exception("BccMonday")));
            }

            return default(T);
        }

        private T Mutation<T>(string query)
        {
            if (_isInitialized)
            {
                _request.AddJsonBody(new { query });
                _request.AddHeader("API-Version", "2023-10");

                var res = _client.Post(_request);

                if (res.StatusCode == HttpStatusCode.OK)
                {
                    var content = res.Content;
                    var queryData = JsonConvert.DeserializeObject<GraphQLResponse<T>>(content);

                    if (queryData.Data != null)
                    {
                        return queryData.Data;
                    }
                    else if (queryData.Errors != null && queryData.Errors.Count > 0)
                    {
                        string errorMessage = queryData.Errors[0].Message;
                        ExceptionLogService.LogException(new Exception($"{errorMessage} | query: {query}", new Exception("BccMonday")));
                    }
                }
                else if (res.StatusCode == HttpStatusCode.InternalServerError)
                {
                    string errorMessage = $"Monday.com is having technical issues. Your API Request did not go through. Query: {query}";
                    ExceptionLogService.LogException(new Exception(errorMessage, new Exception("BccMonday")));
                }
            }

            return default(T);
        }
        # endregion
    }
}
