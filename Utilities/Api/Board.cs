using Newtonsoft.Json;
using System.Collections.Generic;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    /// <inheritdoc/>
    public class Board : IBoard
    {
        /// <inheritdoc/>
        [JsonProperty("id", Required = Required.Always)]
        public long Id { get; set; }

        /// <inheritdoc/>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <inheritdoc/>
        [JsonProperty("columns", ItemConverterType = typeof(ConcreteConverter<IColumn, Column>), NullValueHandling = NullValueHandling.Ignore)]
        public List<IColumn> Columns { get; set; }

        [JsonProperty("items", ItemConverterType = typeof(ConcreteConverter<IItem, Item>), NullValueHandling = NullValueHandling.Ignore)]
        public List<IItem> Items { get; set; }

        /// <summary>
        /// The board's items (rows).
        /// </summary>
        [JsonProperty("items_page")]
        public string ItemsPage { get; set; }

        /// <inheritdoc/>
        public IColumn GetColumn(string id)
        {
            return Columns?.Find(c => c.Id == id);
        }
    }
}
