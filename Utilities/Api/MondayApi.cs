﻿using com.baysideonline.BccMonday.Utilities.Api.Config;
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

        /// <summary>
        /// Archives a group in a specific board.
        /// </summary>
        /// <param name="boardId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IMondayGroup ArchiveGroup(long boardId, string groupId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Archives a board.
        /// </summary>
        /// <param name="boardId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Board ArchiveBoard(long boardId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Change a column's properties
        /// </summary>
        /// <param name="columnId"></param>
        /// <param name="boardId"></param>
        /// <param name="columnProperty"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Column ChangeColumnMetadata(string columnId, long boardId, string columnProperty, string value)
        {
            throw new NotImplementedException();
        }

        public Item ChangeColumnValue(long itemId, string columnId, long boardId, string json)
        {
            throw new NotImplementedException();
        }

        public Item ChangeMultipleColumnValues(long itemId, long boardId, string columnValues, bool createLabelsIfMissing)
        {
            throw new NotImplementedException();
        }

        public Item ClearItemUpdates(long itemId)
        {
            throw new NotImplementedException();
        }

        public Board CreateBoard(string boardName, string description, string boardKind, long folderId, long workspaceId, long templateId,
            List<string> boardOwnerIds, List<string> boardOwnerTeamIds, List<string> boardSubscriberIds, List<string> boardSubscriberTeamIds, bool empty)
        {
            throw new NotImplementedException();
        }

        public Column CreateColumn(long boardId, string title, string description, string columnType, string defaults, string Id, string afterColumnId)
        {
            throw new NotImplementedException();
        }

        public IMondayGroup CreateGroup(long boardId, string groupName, string position, string relativeTo, string positionRelativeMethod)
        {
            throw new NotImplementedException();
        }

        public Item CreateItem(string itemName, long boardId, string groupId, string columnValues, bool createLabelsIfMissing)
        {
            throw new NotImplementedException();
        }

        public Item CreateSubitem(long parentItemID, string itemName, string columnValues, bool createLabelsIfMissing)
        {
            throw new NotImplementedException();
        }

        public Workspace CreateWorkspace(string name, string kind, string description)
        {
            throw new NotImplementedException();
        }

        public Board DeleteBoard(long boardId)
        {
            throw new NotImplementedException();
        }

        public Column DeleteColumn(long boardId, string columnId)
        {
            throw new NotImplementedException();
        }

        public IMondayGroup DeleteGroup(long boardId, string groupId)
        {
            throw new NotImplementedException();
        }

        public Item DeleteItem(long itemID) {  throw new NotImplementedException(); }

        public Update DeleteUpdate(long updateID) { throw new NotImplementedException();}

        public Workspace DeleteWorkspace(long workspaceID) { throw new NotImplementedException();}

        public IMondayGroup DuplicateGroup(long boardId, string groupId, bool addToTop, string groupTitle)
        {
            throw new NotImplementedException();
        }

        public Item DuplicateItem(long boardId, bool withUpdates, long itemId)
        {
            throw new NotImplementedException();
        }

        public Item MoveItemToBoard(long boardId, string groupId, long itemId, string columnsMapping, string subitemsColumnsMapping)
        {
            throw new NotImplementedException();
        }

        public Item MoveItemToGroup() {  throw new NotImplementedException(); }

        public string UpdateBoard() {  throw new NotImplementedException(); }

        public string UpdateGroup() {  throw new NotImplementedException(); }

        public Workspace UpdateWorkspace() {  throw new NotImplementedException(); }

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
