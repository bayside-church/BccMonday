﻿using com.baysideonline.BccMonday.Utilities.Api.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.Utilities.Api.Responses
{
    public class GetItemsResponse
    {
        [JsonProperty("items")]
        public List<Item> Items { get; set; }
    }
}
