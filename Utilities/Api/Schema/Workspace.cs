using com.baysideonline.BccMonday.Utilities.Api.Interfaces;
using Newtonsoft.Json;

namespace com.baysideonline.BccMonday.Utilities.Api.Schema
{
    /// <summary>
    /// A monday.com workspace
    /// </summary>
    public class Workspace : IWorkspace
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
