using Rock.Model;
using System;
using System.Web;
using RestSharp;
using Rock.Data;
using Rock.Web.Cache;
using com.baysideonline.BccMonday.Utilities.Api.Config;
using Newtonsoft.Json;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public class MondayFileApi : IMondayFileApi
    {

        private readonly string MONDAY_FILE_URL = "https://api.monday.com/v2/file";
        private string _apiKey;

        protected IRestClient _client;

        protected IRestRequest _request;

        protected bool _isInitialized = false;

        public MondayInitializeResponse Initialize()
        {
            if(_isInitialized)
            {
                return new MondayInitializeResponse()
                {
                    Message = "Already initialized",
                    Status = MondayStatuses.OK
                };
            }

            using (var context = new RockContext())
            {
                IMondayApiKey keyHelper = new MondayApiKey(context);
                _apiKey = keyHelper.Get();

                if (_apiKey == null)
                {
                    return new MondayInitializeResponse()
                    {
                        Status = MondayStatuses.ERROR,
                        Message = "Unable to obtain monday.com api key"
                    };
                }
            }
            // initialize rest client & rest request
            _client = new RestClient(MONDAY_FILE_URL);
            _request = new RestRequest(Method.POST)
                .AddHeader("Authorization", _apiKey);

            _isInitialized = true;

            return new MondayInitializeResponse()
            {
                Status = MondayStatuses.OK
            };
        }

        #region mutations
        //TODO(Noah): This is unused. Leave it for now because it might be useful if implemented.
        public MondayApiResponse<IAsset> AddFileToColumn(long itemId, string columnId, BinaryFile binaryFile)
        {
            if (!Initialize().IsOk())
            {
                return MondayApiResponse<IAsset>.CreateErrorResponse();
            }

            var filePath = new Uri(GlobalAttributesCache.Get().GetValue("PublicApplicationRoot"))
                + $"GetFile.ashx?id={binaryFile.Id}";
            var fileName = binaryFile.FileName;
            var fileSize = binaryFile.FileSize;
            const long MAX_BYTES = 5000000;

            if (fileSize > MAX_BYTES)
            {
                return null;
            }

            using (System.Net.WebClient webClient = new System.Net.WebClient())
            {
                var bytes = webClient.DownloadData(filePath);
                var query = @"
                        mutation ($file: File!) {
                            add_file_to_update(file: $file, item_id: " + itemId + ", column_id: " + columnId + @") {
                                id
                                file_size
                                name
                                public_url
                                url-thumbnail
                            }
                        }";

                _request.AddHeader("Content-Type", "multipart/form-data");
                _request.AddParameter("query", query);
                _request.AddFile("variables[file]", bytes, fileName);
                var response = _client.Execute(_request);

                return null;
            }
        }

        public MondayApiResponse<IAsset> AddFileToUpdate(long updateId, BinaryFile binaryFile)
        {
            if (!Initialize().IsOk())
            {
                return MondayApiResponse<IAsset>.CreateErrorResponse();
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

            using( System.Net.WebClient webClient = new System.Net.WebClient())
            {
                var bytes = webClient.DownloadData(filePath);
                string query = @"
                        mutation ($file: File!) {
                            add_file_to_update(file: $file, update_id: " + updateId + @") {
                                id
                                file_size
                                name
                                public_url
                                url_thumbnail
                            }
                        }";

                _request.AddHeader("Content-Type", "multipart/form-data");
                _request.AddParameter("query", query);
                _request.AddFile("variables[file]", bytes, fileName);
                var response = _client.Execute(_request);

                if(response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response.Content);
                    //var fileData = (string)data["data"]["add_file_to_update"];
                    var fileData = Convert.ToString(data["data"]["add_file_to_update"]);

                    if (fileData != null)
                    {
                        var uploadedFile = JsonConvert.DeserializeObject<Asset>(fileData);
                        return MondayApiResponse<IAsset>.CreateOkResponse(uploadedFile);
                    }
                }

                return MondayApiResponse<IAsset>.CreateErrorResponse();
            }
        }
        #endregion

        private dynamic GetRootObject(dynamic data, string key)
        {
            if (data != null && data[key] != null && data[key].Count > 0)
                return data[key][0];

            return null;
        }
    }
}
