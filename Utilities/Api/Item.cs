using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    /// <summary>
    /// An item (table row).
    /// </summary>
    public class Item : IItem
    {
        /// <summary>
        /// the item's unique identifier.
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; set; }
        
        /// <summary>
        /// The item's name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        private DateTime _createdAt;

        /// <summary>
        /// the item's create date.
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime CreatedAt
        {
            get => _createdAt.ToLocalTime();
            set => _createdAt = value.ToLocalTime();
        }

        [JsonIgnore]
        public long? BoardId
        {
            get => Board.Id;
            set => BoardId = value;
        }

        /// <summary>
        /// The board that contains this item.
        /// </summary>
        [JsonProperty("board", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        [JsonConverter(typeof(ConcreteConverter<IBoard, Board>))]
        public IBoard Board { get; set; }

        /// <summary>
        /// The item's column values.
        /// </summary>
        [JsonProperty("column_values", ItemConverterType = typeof(Interfaces.ColumnValueConverter))]
        public List<Interfaces.AbstractColumnValue> ColumnValues { get; set; }

        /// <summary>
        /// The item's updates.
        /// </summary>
        [JsonProperty("updates", ItemConverterType = typeof(ConcreteConverter<IUpdate, Update>))]
        public List<IUpdate> Updates { get; set; }

        public string GetRequestorEmail(string columnId)
        {
            var columnValue = this.ColumnValues.FirstOrDefault(c => c.ColumnId == columnId);
            return columnValue?.Text;
        }
    }
}
