using com.baysideonline.BccMonday.Utilities.Api.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public class ItemsPage : IItemsPage
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("cursor", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Cursor { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("items", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ListItemConverter<IItem, Item>))]
        public List<IItem> Items { get; set; }
    }
}
