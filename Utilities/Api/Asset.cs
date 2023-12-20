using Newtonsoft.Json;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    /// <summary>
    /// A file uploaded to <see href="https://monday.com"/>
    /// </summary>
    public class Asset : IAsset
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("file_size")]
        public long Size { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("public_url")]
        public string PublicUrl { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("url_thumbnail")]
        public string UrlThumbnail { get; set; }
    }
}
