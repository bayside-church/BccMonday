using Newtonsoft.Json;
using System;

namespace com.baysideonline.BccMonday.Utilities.Api.Schema
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
        [JsonProperty("created_at", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("file_extension", NullValueHandling = NullValueHandling.Ignore)]
        public int FileExtension { get; set; }

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
        [JsonProperty("original_geometry", NullValueHandling = NullValueHandling.Ignore)]
        public string OriginalGeometry { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("public_url")]
        public string PublicUrl { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("uploaded_by", NullValueHandling = NullValueHandling.Ignore)]
        public MondayUser Uploader { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("url_thumbnail")]
        public string UrlThumbnail { get; set; }
    }
}
