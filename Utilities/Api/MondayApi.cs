using com.baysideonline.BccMonday.Utilities.Api.Config;
using com.baysideonline.BccMonday.Utilities.Api.Interfaces;
using com.baysideonline.BccMonday.Utilities.Api.Responses;
using com.baysideonline.BccMonday.Utilities.Api.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Rock.CheckIn;
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


            
            var query = parentUpdateId != null ?
                new GraphQLQueryBuilder("mutation")
                .AddVariable("$itemId", "ID")
                .AddVariable("$body", "String!")
                .AddVariable("$parentUpdateId", "ID")
                .AddNestedField("create_update",
                    new Dictionary<string, object>
                    {
                        { "item_id", "$itemId" },
                        { "body", "$body" },
                        { "parent_id", "$parentUpdateId" }
                    }
                    , q => q
                    .AddField("id")
                    .AddField("body")
                    .AddField("text_body")
                    .AddField("created_at")
                    .AddField("creator_id")
                    .AddNestedField("creator", q1 => q1
                        .AddField("id")
                        .AddField("name")
                    )
                ).Build()
             : new GraphQLQueryBuilder("mutation")
                .AddVariable("$itemId", "ID")
                .AddVariable("$body", "String!")
                .AddNestedField("create_update",
                    new Dictionary<string, object>
                    {
                        { "item_id", "$itemId" },
                        { "body", "$body" },
                    }
                    , q => q
                    .AddField("id")
                    .AddField("body")
                    .AddField("text_body")
                    .AddField("created_at")
                    .AddField("creator_id")
                    .AddNestedField("creator", q1 => q1
                        .AddField("id")
                        .AddField("name")
                    )
                ).Build();

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

        public StatusColumnValue ChangeColumnValue(ColumnChangeOptions options)
        {
            if (!Initialize().IsOk())
                return null;

            var query = new GraphQLQueryBuilder("mutation")
                .AddVariable("$boardId", "ID!")
                .AddVariable("$columnId", "String!")
                .AddVariable("$itemId", "ID")
                .AddVariable("$newValue", "String")
                .AddNestedField("change_simple_column_value",
                    new Dictionary<string, object>
                    {
                        { "board_id", "$boardId" },
                        { "column_id", "$columnId" },
                        { "item_id", "$itemId" },
                        { "value", "$newValue" }
                    }
                    , q => q
                    .AddField("id")
                    .AddNestedField("column_values",
                        new Dictionary<string, object>
                        {
                            { "ids", "[$columnId]" }
                        }
                        , q1 => q1
                        .AddField("id")
                        .AddField("text")
                        .AddField("type")
                        .AddNestedField("... on StatusValue", q2 => q2
                            .AddNestedField("label_style", q3 => q3
                                .AddField("color")
                            )
                        )
                    )
                ).Build();

            var variables = new Dictionary<string, object>()
            {
                { "boardId", options.BoardId },
                { "columnId", options.ColumnId },
                { "itemId", options.ItemId },
                { "newValue", options.Value }
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

                var query = new GraphQLQueryBuilder("mutation")
                    .AddVariable("$file", "File!")
                    .AddVariable("$updateId", "ID!")
                    .AddNestedField("add_file_to_update",
                        new Dictionary<string, object>
                        {
                            { "update_id", "$updateId" }
                        }
                        , q => q
                        .AddField("id")
                        .AddField("file_size")
                        .AddField("name")
                        .AddField("public_url")
                        .AddField("url_thumbnail")
                    ).Build();

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

        /*
        private Asset AddFileToEntity(Dictionary<string, object> variables, string query, BinaryFile file, Type responseType)
        {
            if (!Initialize().IsOk())
            {
                //Log error or throw exception
                return null;
            }

            var publicApplicationRoot = GlobalAttributesCache.Get().GetValue("PublicApplicationRoot");
            var filePath = new Uri(Path.Combine(publicApplicationRoot, $"GetFile.ashx={file.Id}"));
            var fileName = file.FileName;
            var fileSize = file.FileSize;
            const long MaxBytes = 100_000_000;

            if (!fileSize.HasValue || fileSize.Value > MaxBytes)
            {
                return null;
            }

            using (WebClient webClient = new WebClient())
            {
                var bytes = webClient.DownloadData(filePath);
                variables.Add("file", bytes);

                var data = FileQuery<>(query,)
            }
        }
        */

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
            var query = new GraphQLQueryBuilder("mutation")
                .AddVariable("$boardId", "ID!")
                .AddVariable("$groupId", "String!")
                .AddNestedField("archive_group",
                    new Dictionary<string, object>
                    {
                        { "board_id", "$boardId"  },
                        { "group_id", "$groupId" }
                    }
                    , q => q
                    .AddField("id")
                ).Build();

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
            var query = new GraphQLQueryBuilder("mutation")
                .AddVariable("$boardId", "ID!")
                .AddNestedField("archive_board",
                    new Dictionary<string, object>
                    {
                        { "board_Id", "$boardId"  },
                    }
                    ,q => q
                    .AddField("id")
                ).Build();

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
        public Column ChangeColumnMetadata(ColumnChangeMetadataOptions options)
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
        public Item ChangeMultipleColumnValues(ColumnChangeMultipleValuesOptions options)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clear an item's updates
        /// </summary>
        /// <param name="itemId">The item's unique identifier</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Item ClearItemUpdates(ItemClearUpdatesOptions options)
        {
            var query = new GraphQLQueryBuilder("mutation")
                .AddVariable("$itemId", "ID!")
                .AddNestedField("clear_item_updates",
                    new Dictionary<string, object>
                    {
                        { "item_Id", "$itemId"  },
                    }
                    ,q => q
                    .AddField("id")
                ).Build();

            throw new NotImplementedException();
        }
        #region Create Entities
        public Board CreateBoard(BoardCreationOptions options)
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
        public Column CreateColumn(ColumnCreationOptions options)
        {
            if (!Initialize().IsOk())
                return null;

            var builder = new GraphQLQueryBuilder("mutation")
                .AddVariable("$boardId", "ID!");

            /*
            var query = new GraphQLQueryBuilder("mutation")
                .AddVariable("$boardId", "ID!")
                .AddVariable("$title", "String!")
                .AddVariable("$description", "String")
                .AddVariable("$columnType", "ColumnType!")
                .AddVariable("$defaults", "JSON")
                .AddVariable("$id", "String")
                .AddVariable("$afterColumnId", "ID")
                .AddNestedField("create_column",
                    new Dictionary<string, object>
                    {
                        { "board_id", "$boardId" },
                        { "title", "$title" },
                        { "description", "$description" },
                        { "column_type", "$columnType" },
                        { "defaults", "$defaults" },
                        { "id", "$id" },
                        { "after_column_id", "$afterColumnId" }
                    }
                    , q => q
                    .AddField("id")
                ).Build();
        */

            var boardId = options.BoardId;
            var afterColumnId = options.AfterColumnId;

            //We only want to include properties from options that aren't null.
            // We want to exclude any propertys from options that are null.
            // We need to create a new CreateColumnResponse type that maps to a Column

            var optionsProperties = typeof(ColumnCreationOptions).GetProperties();
            var variables = new Dictionary<string, object>
            {
                { "boardId", options.BoardId }
            };

            var args = new Dictionary<string, object>
            {
                { "board_id", "$boardId" }
            };

            if (options.Title != null)
            {
                variables.Add("title", options.Title);
                builder.AddVariable("$title", "String!");
                args.Add("title", "$title");
            }

            if (options.Description != null)
            {
                variables.Add("description", options.Description);
                builder.AddVariable("$description", "String");
                args.Add("description", "$description");
            }

            if (options.Id != null)
            {
                variables.Add("id", options.Id);
                builder.AddVariable("$id", "String");
                args.Add("id", "$id");
            }

            if (options.AfterColumnId != null)
            {
                variables.Add("afterColumnId", options.AfterColumnId);
                builder.AddVariable("$afterColumnId", "ID");
                args.Add("after_column_id", "$afterColumnId");
            }

            if (options.Defaults != null)
            {
                variables.Add("defaults", options.Defaults);
                builder.AddVariable("$defaults", "JSON");
                args.Add("defaults", "$defaults");
            }

            if (options.ColumnType != null)
            {
                variables.Add("columnType", options.ColumnType);
                builder.AddVariable("$columnType", "ColumnType!");
                args.Add("column_type", "$columnType");
            }

            builder.AddNestedField("create_column", args, q => q
                    .AddField("id")
                );

            var query = builder.Build();

            /*
            var variables = new Dictionary<string, object>
            {
                { "boardId", options.BoardId },
                { "title", options.Title },
                { "description", options.Description },
                { "id", options.Id },
                { "afterColumnId", options.AfterColumnId },
                { "defaults", options.Defaults },
                { "columnType", options.ColumnType }
            };
            */
            var res = Query<CreateColumnResponse>(query, variables);

            if (res == null) return null;
            if (res.Column == null) return null;

            return res.Column;

            //throw new NotImplementedException();
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
            var query = new GraphQLQueryBuilder("mutation")
                .AddVariable("$boardId", "ID!")
                .AddVariable("$groupName", "String!")
                .AddVariable("$relativeTo", "String")
                .AddVariable("$positionRelativeMethod", "PositionRelative")
                .AddNestedField("create_group",
                    new Dictionary<string, object>
                    {
                        { "board_Id", "$boardId"  },
                        { "group_name", "$groupName" },
                        { "relative_to", "$relativeTo" },
                        { "position_relative_method", "$positionRelativeMethod" },
                    }
                    ,q => q
                    .AddField("id")
                ).Build();

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
        public Item CreateItem(ItemCreationOptions options)
        {
            var query = new GraphQLQueryBuilder("mutation")
                .AddVariable("$itemName", "String!")
                .AddVariable("$boardId", "ID!")
                .AddVariable("$groupId", "String")
                .AddVariable("$columnValues", "JSON")
                .AddVariable("$createLabelsIfMissing", "Boolean")
                .AddNestedField("create_item",
                    new Dictionary<string, object>
                    {
                        { "item_name", "$itemName" },
                        { "board_Id", "$boardId"  },
                        { "group_id", "$groupId" },
                        { "column_values", "$columnValues" },
                        { "create_labels_if_missing", "$createLabelsIfMissing" },
                    }
                    , q => q
                    .AddField("id")
                ).Build();

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
        public Item CreateSubitem(SubItemCreationOptions options)
        {
            var query = new GraphQLQueryBuilder("mutation")
                .AddVariable("$parentItemId", "ID!")
                .AddVariable("$itemName", "String!")
                .AddVariable("$columnValues", "JSON")
                .AddVariable("$createLabelsIfMissing", "Boolean")
                .AddNestedField("create_subitem",
                    new Dictionary<string, object>
                    {
                        { "parent_item_id", "$parentItemId" },
                        { "item_name", "$itemName" },
                        { "column_values", "$columnValues" },
                        { "create_labels_if_missing", "$createLabelsIfMissing" },
                    }
                    , q => q
                    .AddField("id")
                ).Build();

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
            var query = new GraphQLQueryBuilder("mutation")
                .AddVariable("$name", "String!")
                .AddVariable("$kind", "WorkspaceKind!")
                .AddVariable("$description", "String")
                .AddNestedField("create_workspace",
                    new Dictionary<string, object>
                    {
                        { "name", "$name" },
                        { "kind", "$kind"  },
                        { "description", "$description" },
                    }
                    , q => q
                    .AddField("id")
                ).Build();

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
            var query = new GraphQLQueryBuilder("mutation")
                .AddVariable("$boardId", "ID!")
                .AddNestedField("delete_board",
                    new Dictionary<string, object>
                    {
                        { "board_id", "$boardId"  },
                    }
                    ,q => q
                    .AddField("id")
                ).Build();

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
            var query = new GraphQLQueryBuilder("mutation")
                .AddVariable("$boardId", "ID!")
                .AddVariable("$columnId", "String!")
                .AddNestedField("delete_column",
                    new Dictionary<string, object>
                    {
                        { "board_id", "$boardId"  },
                        { "column_id", "$columnId" }
                    }
                    , q => q
                    .AddField("id")
                ).Build();

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
            var query = new GraphQLQueryBuilder("mutation")
                .AddVariable("$boardId", "ID!")
                .AddVariable("$groupId", "String!")
                .AddNestedField("delete_group",
                    new Dictionary<string, object>
                    {
                        { "board_id", "$boardId"  },
                        { "group_id", "$groupId" }
                    }
                    , q => q
                    .AddField("id")
                ).Build();

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
            var query = new GraphQLQueryBuilder("mutation")
                .AddVariable("$itemId", "ID")
                .AddNestedField("delete_item",
                    new Dictionary<string, object>
                    {
                        { "item_id", "$itemId"  },
                    }
                    , q => q
                    .AddField("id")
                ).Build();

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
            var query = new GraphQLQueryBuilder("mutation")
                .AddVariable("$updateId", "ID!")
                .AddNestedField("delete_update",
                    new Dictionary<string, object>
                    {
                        { "id", "$updateId"  },
                    }
                    , q => q
                    .AddField("id")
                ).Build();

            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Delete workspace.
        /// </summary>
        /// <param name="workspaceId">The workspace's unique identifier</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Workspace DeleteWorkspace(long workspaceId)
        {
            var query = new GraphQLQueryBuilder("mutation")
                .AddVariable("$workspaceId", "ID!")
                .AddNestedField("delete_workspace",
                    new Dictionary<string, object>
                    {
                        { "workspace_id", "$workspaceId"  },
                    }
                    , q => q
                    .AddField("id")
                ).Build();

            throw new NotImplementedException();
        }
        #endregion

        #region DuplicatEntities
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

            var query = new GraphQLQueryBuilder()
                .AddVariable("$assetIds", "[ID!]")
                .AddNestedField("assets",
                    new Dictionary<string, object>
                    {
                        { "ids", new [] { "$assetIds" } }
                    }
                    ,q => q.AssetProps()
                ).Build();

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

            var query = new GraphQLQueryBuilder()
               .AddVariable("$boardId", "ID!")
               .AddNestedField("boards",
                   new Dictionary<string, object>
                       {
                           { "ids", new[] { "$boardId" } },
                           { "limit", 1 }
                       }
                    , q => q
                   .AddField("id")
                   .AddField("name")
                   .AddField("type")
                   .AddNestedField("columns", q1 => q1
                       .AddField("id")
                       .AddField("title")
                       .AddField("type")
                       .AddField("settings_str")
                   )
               ).Build();

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

            var query = new GraphQLQueryBuilder()
                .AddVariable("$boardId", "ID!")
                .AddNestedField("boards",
                    new Dictionary<string, object>
                    {
                        { "ids", new[] { "$boardId" } }
                    }
                    , q => q
                    .AddField("id")
                    .AddField("name")
                    .AddField("type")
                    .AddNestedField("workspace", q1 => q1
                        .AddField("id")
                        .AddField("name")
                    )
                ).Build();

            var variables = new Dictionary<string, object>()
            {
                { "boardId", boardId }
            };

            var queryData = Query<GetBoardsResponse>(query, variables);
            var board = queryData.Boards[0];
            var workspace = board.Workspace;
            return workspace.Name ?? null;
        }

        public List<Board> GetBoards()
        {
            if (!Initialize().IsOk())
                return null;

            var query = new GraphQLQueryBuilder()
                .AddNestedField("boards",
                    new Dictionary<string, object>
                    {
                        { "limit", 500 }
                    }
                    , q => q
                    .AddField("id")
                    .AddField("name")
                    .AddField("type")
                ).Build();

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

            var query = new GraphQLQueryBuilder()
                .AddVariable("$itemId", "ID!")
                .AddNestedField("items",
                    new Dictionary<string, object>
                    {
                        { "ids", new [] { "$itemId" } },
                        { "limit", 1 }
                    }
                    , q => q
                    .AddField("id")
                    .AddField("name")
                    .AddField("created_at")
                    .AddNestedField("board", q1 => q1
                        .AddField("id")
                        .AddField("name")
                        .AddField("type")
                    )
                    .AddNestedField("column_values", q2 => q2
                        .ColumnValueProps()
                        .FileValueFragment()
                        .EmailValueFragment()
                        .StatusValueFragment()
                        .DateValueFragment()
                        .BoardRelationValueFragment()
                        .ButtonValueFragment()
                        .MirrorValueFragment()
                        .AddNestedField("column", qc => qc.ColumnProps())
                    )
                    .AddNestedField("updates", q16 => q16
                        .UpdateProps()
                        .AddNestedField("replies", q18 => q18.UpdateProps() )
                        .AddNestedField("assets", q20 => q20.AssetProps() )
                    )
                ).Build();

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

        public List<Item> GetItemsByBoardAndColumnValues(long boardId, List<ItemsPageByColumnValuesQuery> columnValues)
        {
            if (!Initialize().IsOk()) return null;

            List<Item> allItems = new List<Item>();

            var query = new GraphQLQueryBuilder()
                .AddVariable("$boardId", "ID!")
                .AddVariable("$columnValue", "String!")
                .AddVariable("$columnId", "String!")
                .AddNestedField("items_page_by_column_values",
                    new Dictionary<string, object>
                    {
                        { "limit", 500 },
                        { "board_id", "$boardId" },
                        { "columns", new List<Dictionary<string, object>>
                            {
                                new Dictionary<string, object>
                                {
                                    { "column_id", "$columnId" },
                                    { "column_values", new List<string> { "$columnValue" } }
                                }
                            }
                        }
                    }
                    , q => q
                    .AddField("cursor")
                    .AddNestedField("items", q1 => q1
                        .AddField("id")
                        .AddField("name")
                        .AddField("created_at")
                        .AddNestedField("column_values", q2 => q2
                            .ColumnValueProps()
                            .AddNestedField("column", q3 => q3.ColumnProps() )
                            .MirrorValueFragment()
                            .StatusValueFragment()
                        )
                    )
                ).Build();

            /*
             // Convert the list of objects to a list of dictionaries
List<Dictionary<string, object>> columnsList = itemsList.Select(item =>
    new Dictionary<string, object>
    {
        { "column_id", item.ColumnId },
        { "column_values", item.ColumnValues }
    }).ToList();
            */

            var variables = new Dictionary<string, object>()
            {
                { "boardId", boardId },
                { "columnValue", columnValues[0].ColumnValues[0] },
                { "columnId", columnValues[0].ColumnId }
            };

            var initialData = Query<GetItemsByPageResponse>(query, variables);
            if (initialData == null) return allItems;
            if (initialData.ItemsPage == null) return allItems;
            var itemsPage = initialData.ItemsPage;
            if (itemsPage.Items == null) return allItems;
            var items = itemsPage.Items.ConvertAll(i => (Item)i);
            //var cursor = initialData.Cursor;
            allItems.AddRange(items);

            return allItems;
        }

        public List<Item> GetItemsByBoard(long boardId, string emailMatchColumnId, string statusColumnId)
        {
            if (!Initialize().IsOk())
                return null;

            List<Item> allItems = new List<Item>();

            var query = new GraphQLQueryBuilder()
                .AddVariable("$boardId", "ID!")
                .AddVariable("$emailColumnId", "String!")
                .AddVariable("$statusColumnId", "String!")
                .AddNestedField("boards",
                    new Dictionary<string, object>
                    {
                        { "ids", new [] { "$boardId" } },
                        { "limit", 1 },
                    }
                    , q => q
                    .AddField("id")
                    .AddNestedField("items_page", q1 => q1
                        .AddField("cursor")
                        .AddNestedField("items", q2 => q2
                            .AddField("id")
                            .AddField("name")
                            .AddField("created_at")
                            .AddNestedField("column_values",
                                new Dictionary<string, object>
                                {
                                    { "ids", new[] { "$emailColumnId", "$statusColumnId" } }
                                }
                                , q3 => q3
                                .ColumnValueProps()
                                .AddNestedField("column", q4 => q4.ColumnProps())
                                .MirrorValueFragment()
                                .StatusValueFragment()
                            )
                        )
                    )
                ).Build();

            var variables = new Dictionary<string, object>()
            {
                { "boardId", boardId },
                { "emailColumnId", emailMatchColumnId },
                { "statusColumnId", statusColumnId }
            };
            var initialQueryData = Query <GetBoardsResponse> (query, variables);
            if (initialQueryData == null) return allItems;
            if (initialQueryData.Boards == null) return allItems;
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
                var nextItemsQuery = new GraphQLQueryBuilder()
                    .AddVariable("cursorVal", "String")
                    .AddVariable("emailColumnId", "String!")
                    .AddVariable("statusColumnId", "String!")
                    .AddNestedField("next_items_page",
                        new Dictionary<string, object>
                        {
                            { "cursor", "$cursorVal" },
                            { "limit", 10 }
                        }
                        , q => q
                        .AddField("cursor")
                        .AddNestedField("items", q1 => q1
                            .AddField("id")
                            .AddField("name")
                            .AddField("created_at")
                            .AddNestedField("column_values", q2 => q2
                                .ColumnValueProps()
                                .AddNestedField("column", q3 => q3.ColumnProps() )
                                .StatusValueFragment()
                                .MirrorValueFragment()
                            )
                        )
                    ).Build();

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
