﻿using System.Web.UI;
using com.baysideonline.BccMonday.Utilities.Api.Schema;
using Newtonsoft.Json;

namespace com.baysideonline.BccMonday.Utilities.Api.Interfaces
{
    [JsonConverter(typeof(ColumnValueConverter))]
    public class AbstractColumnValue
    {
        /// <summary>
        /// the column that this value belongs to.
        /// </summary>
        [JsonProperty("id")]
        public string ColumnId { get; set; }

        /// <summary>
        /// Text representation of the column value. Note: Not all columns support textual value
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        /// The column's type.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// The column's raw value in JSON format.
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }

        /// <summary>
        /// The column that this value belongs to.
        /// </summary>
        [JsonProperty("column")]//, ItemConverterType = typeof(ConcreteConverter<IColumn, Column>))]
        public Column Column { get; set; }

        public virtual Control CreateControl(Page page)
        {
            var root = new LiteralControl();
            return root;
        }
    }
}
