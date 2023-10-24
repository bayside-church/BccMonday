using Newtonsoft.Json;
using System.Collections.Generic;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public class Board : IBoard
    {
        [JsonProperty("id", Required = Required.Always)]
        public long Id { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("columns", ItemConverterType = typeof(ConcreteConverter<IColumn, Column>), NullValueHandling = NullValueHandling.Ignore)]
        public List<IColumn> Columns { get; set; }

        [JsonProperty("items", ItemConverterType = typeof(ConcreteConverter<IItem, Item>), NullValueHandling = NullValueHandling.Ignore)]
        public List<IItem> Items { get; set; }


        public IColumn GetColumn(string id)
        {
            return Columns?.Find(c => c.Id == id);
        }
    }
}
