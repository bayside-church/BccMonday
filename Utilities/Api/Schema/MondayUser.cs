using com.baysideonline.BccMonday.Utilities.Api.Interfaces;
using Newtonsoft.Json;

namespace com.baysideonline.BccMonday.Utilities.Api.Schema
{
    /// <summary>
    /// A monday.com user.
    /// </summary>
    public class MondayUser : IMondayUser
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("id")]
        public string CreatorId { get; set; }
    }
}
