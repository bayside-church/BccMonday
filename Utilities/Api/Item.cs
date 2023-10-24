using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public class Item : IItem
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        private DateTime _createdAt;

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

        [JsonProperty("board", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        [JsonConverter(typeof(ConcreteConverter<IBoard, Board>))]
        public IBoard Board { get; set; }

        [JsonProperty("column_values", ItemConverterType = typeof(Interfaces.ColumnValueConverter))]
        public List<Interfaces.AbstractColumnValue> ColumnValues { get; set; }

        [JsonProperty("updates", ItemConverterType = typeof(ConcreteConverter<IUpdate, Update>))]
        public List<IUpdate> Updates { get; set; }

        public string GetRequestorEmail(string columnId)
        {
            var columnValue = this.ColumnValues.FirstOrDefault(c => c.ColumnId == columnId);
            return columnValue?.Text;
        }
    }
}
