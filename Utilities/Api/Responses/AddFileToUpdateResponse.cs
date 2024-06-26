﻿using com.baysideonline.BccMonday.Utilities.Api.Schema;
using Newtonsoft.Json;

namespace com.baysideonline.BccMonday.Utilities.Api.Responses
{
    /// <summary>
    /// Add a file to an update Response
    /// </summary>
    public class AddFileToUpdateResponse
    {
        [JsonProperty("add_file_to_update")]
        public Asset Asset { get; set; }
    }
}
