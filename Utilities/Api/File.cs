using Newtonsoft.Json;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public class File : IFile
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("file_size")]
        public long Size { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("public_url")]
        public string PublicUrl { get; set; }

        [JsonProperty("url_thumbnail")]
        public string UrlThumbnail { get; set; }
    }
}
