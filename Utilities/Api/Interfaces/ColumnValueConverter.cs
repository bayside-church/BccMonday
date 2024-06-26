﻿using System;
using System.Collections.Generic;
using com.baysideonline.BccMonday.Utilities.Api.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rock.UniversalSearch;

namespace com.baysideonline.BccMonday.Utilities.Api.Interfaces
{
    public class ColumnValueConverter : JsonConverter<AbstractColumnValue>
    {
        private readonly Dictionary<string, Type> columnTypeMappings;

        public ColumnValueConverter()
        {
            columnTypeMappings = new Dictionary<string, Type>
        {
            {"status", typeof(StatusColumnValue)},
            {"file", typeof(FileColumnValue)},
            {"board-relation", typeof(BoardRelationColumnValue)},
            // Add other mappings here
        };
        }

        public override AbstractColumnValue ReadJson(JsonReader reader, Type objectType, AbstractColumnValue existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);
            if (jsonObject.Count == 0)
            {
                // You can return null or a default value here, depending on your requirements.
                return null;
            }

            string type = jsonObject["type"].ToString();

            if (columnTypeMappings.TryGetValue(type, out Type concreteType))
            {
                AbstractColumnValue columnValue = (AbstractColumnValue)Activator.CreateInstance(concreteType);
                serializer.Populate(jsonObject.CreateReader(), columnValue);
                columnValue.Text = jsonObject["text"]?.Value<string>() ?? jsonObject["display_value"]?.Value<string>();
                return columnValue;
            }
            else
            {
                var columnValue = new ColumnValue();
                serializer.Populate(jsonObject.CreateReader(), columnValue);
                columnValue.Text = jsonObject["text"]?.Value<string>() ?? jsonObject["display_value"]?.Value<string>();
                return columnValue;
                //throw new JsonSerializationException($"Unknown type: {type}");
            }
        }

        public override void WriteJson(JsonWriter writer, AbstractColumnValue value, JsonSerializer serializer)
        {
//            var jsonObject = JObject.FromObject(value);
//            jsonObject.WriteTo(writer);
            serializer.Serialize(writer, value, value.GetType());
        }
    }
}
