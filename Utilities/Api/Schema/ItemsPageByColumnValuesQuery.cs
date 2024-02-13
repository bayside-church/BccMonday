using com.baysideonline.BccMonday.Utilities.Api.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace com.baysideonline.BccMonday.Utilities.Api.Schema
{
    public class ItemsPageByColumnValuesQuery : IItemsPageByColumnValuesQuery
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("column_id")]
        public string ColumnId { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("column_values")]
        public List<string> ColumnValues { get; set; }

        public static string SerializeAsTemplate(List<ItemsPageByColumnValuesQuery> items)
        {
            var templateItems = items.ConvertAll(item => new
            {
                column_id = EscapeValue(item.ColumnId),
                column_values = item.ColumnValues.Select(EscapeValue).ToList()
            });

            using (var stringWriter = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(stringWriter) { QuoteName = false })
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(jsonWriter, templateItems);
                return stringWriter.ToString();
            }
        }

        private static string EscapeValue(string value)
        {
            // Implement your custom escaping logic here if needed
            return value.Replace("\"", "\\\"");
        }
    }
}
