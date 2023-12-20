﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.Utilities.Api.Responses
{
    /// <summary>
    /// Add a file to an update Response
    /// </summary>
    public class AddFileToUpdateResponse
    {
        [JsonProperty("add_file_to_update", ItemConverterType = typeof(ConcreteConverter<IAsset, Asset>))]
        public IAsset Asset { get; set; }
    }
}
