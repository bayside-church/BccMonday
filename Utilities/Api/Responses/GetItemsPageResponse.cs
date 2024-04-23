using com.baysideonline.BccMonday.Utilities.Api.Schema;
using Newtonsoft.Json;

namespace com.baysideonline.BccMonday.Utilities.Api.Responses
{
    public class GetItemsPageResponse
    {
        [JsonProperty("items_page")]
        public ItemsPage ItemsPage { get; set; }
    }
}
