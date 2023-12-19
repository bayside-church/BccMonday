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

        /// <summary>
        /// The board's items (rows).
        /// </summary>
        [JsonProperty("items_page")]
        public ItemsPage ItemsPage { get; set; }

        [JsonProperty("type")]
        public string BoardType { get; set; }

        [JsonProperty("workspace", NullValueHandling = NullValueHandling.Ignore)]
        public Workspace Workspace { get; set; }

        /// <inheritdoc/>
        public IColumn GetColumn(string id)
        {
            return Columns?.Find(c => c.Id == id);
        }
    }

    public class ItemsPage
    {
        /// <summary>
        /// An opaque cursor that represents the position in the list after the last returned item.
        /// Use this cursor for pagination to fetch the next set of items.
        /// If the cursor is null, there are no more items to fetch.
        /// </summary>
        [JsonProperty("cursor", NullValueHandling = NullValueHandling.Ignore)]
        public string Cursor { get; set; }

        /// <summary>
        /// The items associated with the cursor.
        /// </summary>
        [JsonProperty("items", ItemConverterType = typeof(ConcreteConverter<IItem, Item>), NullValueHandling = NullValueHandling.Ignore)]
        public List<IItem> Items { get; set; }
    }

    public class Workspace
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
