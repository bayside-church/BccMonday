using com.baysideonline.BccMonday.Utilities.Api.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    /// <summary>
    /// A <see href="https://monday.com"/> board
    /// </summary>
    public class Board : IBoard
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("id", Required = Required.Always)]
        public long Id { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

         /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("columns", ItemConverterType = typeof(ConcreteConverter<IColumn, Column>), NullValueHandling = NullValueHandling.Ignore)]
        public List<IColumn> Columns { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("items_page")]
        public IItemsPage ItemsPage { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("type")]
        public string BoardType { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("workspace", NullValueHandling = NullValueHandling.Ignore)]
        public IWorkspace Workspace { get; set; }

        /// <inheritdoc/>
        public IColumn GetColumn(string id)
        {
            return Columns?.Find(c => c.Id == id);
        }
    }
}
