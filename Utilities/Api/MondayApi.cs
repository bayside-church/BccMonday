using com.baysideonline.BccMonday.Utilities.Api.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Rock.Data;
using Rock.Model;
using System;
using System.Collections.Generic;
using System.Linq;

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
                ? "mutation {" +
                  "create_update (item_id: " + itemId + ", body: \"" + body + "\", parent_id: " + parentUpdateId.Value +
                  @") {
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
                : "mutation {" +
                  "create_update (item_id: " + itemId + ", body: \"" + body + "\"" + @") {
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

            var queryData = Query(query);
            var updateStr = Convert.ToString(queryData["create_update"]);

            if (updateStr != null)
            {
                var update = JsonConvert.DeserializeObject<Update>(updateStr);
                return update;
            }

            return null;
        }

        //TODO(Noah): This API call returns an Item, not a Column Value.
        //  Return a bool? Or the item singled out to the column value.
        public bool ChangeColumnValue(long boardId, long itemId, string columnId, string newValue)
        {
            if (!Initialize().IsOk())
                return false;

            string query = "mutation { change_simple_column_value(board_id: " + boardId.ToString() + " column_id: \"" + columnId + "\" item_id:" + itemId.ToString() + " value: \"" + newValue + "\") { id } }";

            var queryData = Query(query);
            //var data = GetRootObject(queryData, "change_simple_column_value");
            var data = queryData["change_simple_column_value"];

            if (data != null)
            {
                // TODO: build a new column value based on changed value

                return true;
            }

            return false;
        }

        #endregion

        #region queries

        public List<IFile> GetFilesByAssetIds(List<long> ids)
        {
            if (!Initialize().IsOk())
                return null; MondayApiResponse<List<IFile>>.CreateErrorResponse();

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

            var queryData = Query(query);
            //GetRootObject
            if (queryData["assets"] != null && queryData["assets"].Count > 0)
            {
                string fileStr = Convert.ToString(queryData["assets"]);

                var files = JsonConvert.DeserializeObject<List<File>>(fileStr);
                if (files != null && files.Any())
                {
                    return files.ConvertAll(o => (IFile)o);
                }
            }

            return null;
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

            var queryData = Query(query);
            //TODO(Noah): First of many (but really only one)
            //GetRootObject
            if (queryData["boards"] != null && queryData["boards"].Count > 0)
            {
                string boardStr = Convert.ToString(queryData["boards"][0]);

                var board = JsonConvert.DeserializeObject<Board>(boardStr);

                if (board != null)
                {
                    return board;
                }
            }

            return null;
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
            var queryData = Query(query);
            var data = GetRootObject(queryData, "boards");

            if (data != null)
            {
                return data["workspace"]["name"].Value;
            }

            return null;
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

            var queryData = Query(query);

            if (queryData["boards"] != null && queryData["boards"].Count > 0)
            {
                var boardJArray = new JArray();
                foreach (var board in queryData["boards"])
                {
                    if (string.Equals(board["type"].ToString(), "board"))
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
                    items(ids: [" + id + @"], limit:1) {
                        id
                        name
                        created_at
                        board {
                            id
                            name
				            columns {
                                id
                                type
                                title
                                settings_str
                            }
    	                }
                        column_values {
                            id
                            text
                            title
                            type
                            value
                            additional_info
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

            var queryData = Query(query);
            //GetRootObject
            if (queryData["items"] != null && queryData["items"].Count > 0)
            {
                var itemDetailStr = Convert.ToString(queryData["items"][0]);
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

            var query = @"
                query {
                    boards(ids: " + boardId + @", limit: 1) {
                        id
                        items (limit: 1000){
                            id
                            name
                            created_at
                            column_values(ids: [" + "\"" + emailMatchColumnId + "\",\"" + statusColumnId + "\"" + @"]) {
                                id
                                title
                                text
                                type
                                value
                                additional_info
                            }
                        }
                    }
                }";

            var new_query = @"
                query {
                    boards(ids: " + boardId + @", limit: 1) {
                        id
                        items_page( limit: 500) {
                            cursor
                            items {
                                id
                                name
                                created_at
                                column_values(ids: [" + "\"" + emailMatchColumnId + "\",\"" + statusColumnId + "\"" + @"]) {
                                    id
                                    title
                                    text
                                    type
                                    value
                                    additional_info
                            }
                        }
                    }
                }";
            var newQueryData = Query(new_query);
            dynamic boards = newQueryData["boards"];
            dynamic items_page = boards["items_page"];
            string cursor = items_page["cursor"];


            var queryData = Query(query);
            if (queryData["boards"] != null && queryData["boards"].Count > 0)
            {
                string boardStr = Convert.ToString(queryData["boards"][0]);
                var board = JsonConvert.DeserializeObject<Board>(boardStr);

                if (board != null && board.Items != null)
                {
                    var items = board.Items;
                    return items;
                }
            }

            return null;
        }

        #endregion
        #region private methods

        private dynamic Query(string query)
        {
            if (_isInitialized)
            {
                _request.AddJsonBody(new { query });

                var res = _client.Post(_request);

                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var content = res.Content;
                    var queryData = JsonConvert.DeserializeObject<dynamic>(content);

                    if (queryData["data"] != null)
                    {
                        return queryData["data"];
                    }
                    else
                    {
                        string errorMessage = queryData["errors"][0]["message"].Value;
                        ExceptionLogService.LogException(new Exception(string.Format("{0} | query: {1}", errorMessage, query), new Exception("BccMonday")));
                        return null;
                    }
                }else if (res.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    string errorMessage = string.Format("Monday.com is having technical issues. Your API Request did not go through. Query: {0}", query);
                    ExceptionLogService.LogException(new Exception(errorMessage, new Exception("BccMonday")));
                }
            }

            return null;
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
}
