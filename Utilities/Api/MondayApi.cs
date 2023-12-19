using com.baysideonline.BccMonday.Utilities.Api.Config;
using com.baysideonline.BccMonday.Utilities.Api.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Rock.Data;
using Rock.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public class MondayApi : IMondayApi
    {
        private readonly string MONDAY_API_URL = "https://api.monday.com/v2";

        private string _apiKey;

        protected IRestClient _client;

        protected IRestRequest _request;

        protected bool _isInitialized = false;

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
            _client = new RestClient(MONDAY_API_URL);
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

        public IUpdate AddUpdateToItem(long itemId, string body, long? parentUpdateId = null)
        {
            if (!Initialize().IsOk())
                return null;

            var query = string.Empty;

            query = parentUpdateId != null
                ? @"mutation {
                        create_update (item_id: " + itemId + ", body: \"" + body + "\", parent_id: " + parentUpdateId.Value + @") {
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
                : @"mutation {
                        create_update (item_id: " + itemId + ", body: \"" + body + "\"" + @") {
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

            var queryData = Query<CreateUpdateResponse>(query);
            var update = queryData.Update;
            return update;
        }

        public bool ChangeColumnValue(long boardId, long itemId, string columnId, string newValue)
        {
            if (!Initialize().IsOk())
                return false;

            string query = "mutation { change_simple_column_value(board_id: " + boardId.ToString() + " column_id: \"" + columnId + "\" item_id:" + itemId.ToString() + " value: \"" + newValue + "\") { id } }";
            var queryData = Query<ChangeSimpleColumnValueResponse>(query);
            var item = queryData.Item;

            return item != null;
        }

        #endregion

        #region queries

        public List<IAsset> GetFilesByAssetIds(List<long> ids)
        {
            if (!Initialize().IsOk())
                return null;
            //MondayApiResponse<List<IAsset>>.CreateErrorResponse();

            var query = @"
                query {
                    assets (ids: [" + string.Join(",", ids) + @"]) {
                        id
                        public_url
                        name
                        file_size
                        url_thumbnail
                    }
                }";

            var queryData = Query<List<IAsset>>(query);
            if (queryData.Count <= 0 ) return null;
            return queryData;
        }

        public IBoard GetBoard(long id)
        {
            if (!Initialize().IsOk())
                return null;

            var query = @"
                query {
                    boards (ids:" + id + @" limit:1) {
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

            var queryData = Query<List<IBoard>>(query);

            if (queryData.Count <= 0) return null;
            var board = queryData[0];
            return board;
        }

        public string GetWorkspace(long boardId)
        {
            if (!Initialize().IsOk())
            {
                return null;
            }
            var query = @"
                query {
                    boards(ids:" + boardId + @" ) {
                        workspace {
                            name
                        }
                    }
                }";
            var queryData = Query<GetBoardWorkspaceResponse>(query);
            var board = queryData.Boards[0];
            var workspace = board.Workspace;
            return workspace.Name ?? null;
        }

        public List<IBoard> GetBoards()
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

            var queryData = Query<List<IBoard>>(query);

            if (queryData != null && queryData.Count > 0)
            {
                var boardJArray = new JArray();
                foreach (var board in queryData)
                {
                    if (string.Equals(board.BoardType.ToString(), "board"))
                    {
                        boardJArray.Add(board);
                    }
                }

                string boardArrayStr = Convert.ToString(boardJArray);
                var boards = JsonConvert.DeserializeObject<List<Board>>(boardArrayStr);

                if (boards != null && boards.Any())
                {
                    return boards.ConvertAll(o => (IBoard)o);
                }
            }
            return null;
        }

        public IItem GetItem(long id)
        {
            if (!Initialize().IsOk())
                return null;

            var query = @"
                query {
                  items(ids: [" + id.ToString() + @"], limit: 1) {
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

            var queryData = Query<List<IItem>>(query);
            //GetRootObject
            if (queryData != null && queryData.Count > 0)
            {
                var itemDetailStr = Convert.ToString(queryData[0]);
                var item = JsonConvert.DeserializeObject<Item>(itemDetailStr);

                if (item != null)
                {
                    return item;
                }
            }

            return null;
        }

        public List<IItem> GetItemsByBoard(long boardId, string emailMatchColumnId, string statusColumnId)
        {
            if (!Initialize().IsOk())
                return null;

            List<IItem> allItems = new List<IItem>();

            // Initial query
            var initialQuery = @"
    query {
        boards(ids: " + boardId + @", limit: 1) {
            id
            items_page(limit: 20) {
                cursor
                items {
                    id
                    name
                    created_at
                    column_values(ids: [" + "\"" + emailMatchColumnId + "\",\"" + statusColumnId + "\"" + @"]) {
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
        }
    }";

            //var initialQueryData = Query(initialQuery);
            var initialQueryData = Query <GetBoardsResponse> (initialQuery);
            var board = initialQueryData.Boards[0];
            var itemsPage = board.ItemsPage;
            string cursor = itemsPage.Cursor;

            // Process initial items
            var initialItems = itemsPage.Items;
            if (initialItems != null)
            {
                allItems.AddRange(initialItems);
            }

            while (!string.IsNullOrEmpty(cursor))
            {
                var nextItemsQuery = @"
    query {
      next_items_page(
        cursor: ""MSwxODE5OTg2MDMxLF8wRkhvZzVPU29jLVRoRE5aZl93Tyw1NywyMCx8MzYxNTIzNDk1Mw""
        limit: 500
      ) {
        cursor
        items {
          id
          name
          created_at
          column_values(ids: [""text"", ""status""]) {
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
    }
}";
                    /*@"
        query {
            next_items_page(cursor: """ + cursor + @""", limit: 500) {
                cursor
                items {
                    id
                    name
                    created_at
                    column_values(ids: [""" + emailMatchColumnId + "\",\"" + statusColumnId + @"""]) {
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
        }";*/

                //var nextQueryData = Query(nextItemsQuery);
                var nextItemsPageData = Query<GetNextItemsPageResponse>(nextItemsQuery);
                if (nextItemsPageData == null) return allItems;
                var nextItemsPage = nextItemsPageData.NextItemsPage;
                cursor = nextItemsPage.Cursor;

                var nextItems = nextItemsPage.Items;
                allItems.AddRange(nextItems);
            }

            return allItems;
        }

        #endregion
        #region private methods
        private T Query<T>(string query)
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

            return default;
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

        private dynamic GetRootObject(dynamic data, string key)
        {
            if (data != null && data[key] != null)
            {
                if (data[key].GetType() == typeof(JArray))
                {
                    if (((JArray)data[key]).Count > 0) return data[key][0];
                }
                else
                {
                    if (((JObject)data[key]).Count > 0) return data[key][0];
                }
            }

            return null;
        }

        private dynamic GetRootObjectList(dynamic data, string key)
        {
            if (data != null && data[key] != null)
            {
                if (data[key].GetType() == typeof(JArray))
                {
                    if (((JArray)data[key]).Count > 0) return data[key];
                }
                else
                {
                    if (((JObject)data[key]).Count > 0) return data[key];
                }
            }

            return null;
        }

        # endregion
    }

    public class GraphQLResponse<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }

        [JsonProperty("errors")]
        public List<GraphQlError> Errors { get; set; }
    }

    public class GraphQLMutation<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }

        [JsonProperty("errors")]
        public List<GraphQlError> Errors { get; set; }
    }

    public class GraphQlError
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        // Add other properties of the error object if needed
    }
}
