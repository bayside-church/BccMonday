using System.Web.UI;
using Newtonsoft.Json;

namespace com.baysideonline.BccMonday.Utilities.Api.Interfaces
{
    public abstract class AbstractColumnValue
    {
        [JsonProperty("id")]
        public string ColumnId { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("additional_info")]
        public string AdditionalInfo { get; set; }

        public abstract Control CreateControl(Page page);
    }
}
