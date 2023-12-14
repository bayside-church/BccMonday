using Newtonsoft.Json;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public class Asset : IAsset
    {
        /// <inheritdoc/>
        [JsonProperty("id")]
        public long Id { get; set; }

        /// <inheritdoc/>
        [JsonProperty("file_size")]
        public long Size { get; set; }

        /// <inheritdoc/>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <inheritdoc/>
        [JsonProperty("public_url")]
        public string PublicUrl { get; set; }

        /// <inheritdoc/>
        [JsonProperty("url_thumbnail")]
        public string UrlThumbnail { get; set; }
    }
}
