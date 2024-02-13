using com.baysideonline.BccMonday.Utilities.Api.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.Utilities.Api.Responses
{
    public class GetItemsByPageResponse
    {
        [JsonProperty("items_page_by_column_values")]
        public ItemsPage ItemsPage { get; set; }
    }
}
