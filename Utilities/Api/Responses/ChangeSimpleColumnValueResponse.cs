using com.baysideonline.BccMonday.Utilities.Api.Schema;
using Newtonsoft.Json;

namespace com.baysideonline.BccMonday.Utilities.Api.Responses
{
    /// <summary>
    /// Change an item's column with simple value Response
    /// </summary>
    public class ChangeSimpleColumnValueResponse
    {
        [JsonProperty("change_simple_column_value")]
        public Item Item { get; set; }
    }
}
