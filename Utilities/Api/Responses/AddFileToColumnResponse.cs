using com.baysideonline.BccMonday.Utilities.Api.Schema;
using Newtonsoft.Json;

namespace com.baysideonline.BccMonday.Utilities.Api.Responses
{
    /// <summary>
    /// Add a file to a column Response
    /// </summary>
    public class AddFileToColumnResponse
    {
        [JsonProperty("add_file_to_column")]
        public Asset Asset { get; set; }
    }
}
