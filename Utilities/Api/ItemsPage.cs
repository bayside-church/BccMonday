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
        [JsonProperty("cursor", NullValueHandling = NullValueHandling.Ignore)]
        public string Cursor { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("items", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ConcreteConverter<IItem, Item>))]
        public List<IItem> Items { get; set; }
    }
}
