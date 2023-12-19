using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.Utilities.Api.Responses
{
    public  class GetNextItemsPageResponse
    {
        [JsonProperty("next_items_page")]
        public ItemsPage NextItemsPage { get; set; }
    }
}
